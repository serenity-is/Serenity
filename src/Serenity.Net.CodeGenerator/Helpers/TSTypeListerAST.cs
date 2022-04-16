using Sdcb.TypeScript;
using Sdcb.TypeScript.TsParser;
using Sdcb.TypeScript.TsTypes;
using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class TSTypeListerAST
    {
        private readonly List<ITypeScriptAST> files = new();

        public void AddInputFile(string path, string content)
        {
            files.Add(new TypeScriptAST(content, path));
        }

        private static bool HasExportModifier(INode node)
        {
            return node.Modifiers != null && node.Modifiers.Any(x => x.Kind == SyntaxKind.ExportKeyword);
        }

        private static bool HasDeclareModifier(INode node)
        {
            return node.Modifiers != null && node.Modifiers.Any(x => x.Kind == SyntaxKind.DeclareKeyword);
        }

        private static IEnumerable<INode> EnumerateParents(INode node)
        {
            if (node == null)
                yield break;

            while ((node = node.Parent) != null)
                yield return node;
        }

        private static string GetNamespace(INode node)
        {
            return string.Join(".", EnumerateParents(node)
                .Where(x => x.Kind == SyntaxKind.ModuleDeclaration)
                .Select(x => (x as ModuleDeclaration).Name.GetText())
                .Reverse());
        }

        private static string PrependNamespace(string s, INode node)
        {
            var ns = GetNamespace(node);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + s;
            return s;
        }

        static bool IsParseTreeNode(INode node)
        {
            return (node.Flags & NodeFlags.Synthesized) == 0;
        }

        static INode GetParseTreeNode(INode node, Func<INode, bool> nodeTest = null)
        {
            if (node == null || IsParseTreeNode(node))
                return node;

            node = node.Original;
            while (node != null)
            {
                if (IsParseTreeNode(node))
                    return nodeTest == null || nodeTest(node) ? node : null;

                node = node.Original;
            }

            return null;
        }

        private class TSType
        {
        }

        static readonly TSType ErrorType = new TSType();

        static string GetExpandedExpression(INode node)
        {
            if (node == null)
                return string.Empty;

            return node.GetText();
        }

        static string GetBaseType(ClassDeclaration node)
        {
            if (node.HeritageClauses == null)
                return null;

            foreach (var heritage in node.HeritageClauses) 
            {
                if (heritage.Token == SyntaxKind.ExtendsKeyword &&
                    heritage.Types != null) 
                {
                    foreach (var type in heritage.Types) 
                        return GetExpandedExpression(type);
                }
            }

            return null;
        }

        static List<string> GetInterfaces(ClassDeclaration node)
        {
            var result = new List<string>();
            if (node.HeritageClauses == null)
                return result;

            foreach (var heritage in node.HeritageClauses) 
            {
                if (heritage.Token == SyntaxKind.ImplementsKeyword &&
                    heritage.Types != null)
                {
                    foreach (var type in heritage.Types)
                        result.Add(GetExpandedExpression(type));
                }
            }

            return result;
        }

        static List<string> GetBaseInterfaces(InterfaceDeclaration node)
        {
            var result = new List<string>();
            if (node.HeritageClauses == null)
                return result;

            foreach (var heritage in node.HeritageClauses)
            {
                if (heritage.Token == SyntaxKind.ExtendsKeyword &&
                    heritage.Types != null)
                {
                    foreach (var type in heritage.Types)
                        result.Add(GetExpandedExpression(type));
                }
            }

            return result;
        }

        static List<ExternalGenericParameter> TypeParametersToExternal(NodeArray<TypeParameterDeclaration> p) 
        {
            if (p == null || p.Count == 0)
                return null;

            var result = new List<ExternalGenericParameter>();

            foreach (var k in p)
            {
                result.Add(new ExternalGenericParameter
                {
                    Name = k.GetText()
                });
            }

            return result;
        }

        static bool IsUnderAmbientNamespace(INode node) 
        {
            return EnumerateParents(node).Any(x => 
                (x.Kind == SyntaxKind.ModuleDeclaration &&
                 x.Modifiers != null &&
                 x.Modifiers.Any(z => z.Kind == SyntaxKind.DeclareKeyword)) ||
                (x.Kind == SyntaxKind.SourceFile &&
                 (x as SourceFile).IsDeclarationFile));
        }

        static ExternalAttribute DecoratorToExternalAttribute(Decorator decorator)
        {
            if (decorator.Expression == null)
                return null;

            var result = new ExternalAttribute
            {
                Arguments = new List<ExternalArgument>()
            };

            PropertyAccessExpression pae;
            if (decorator.Expression.Kind == SyntaxKind.CallExpression)
            {
                var ce = decorator.Expression as CallExpression;
                if (ce.Expression != null &&
                    ce.Expression.Kind == SyntaxKind.PropertyAccessExpression)
                {
                    pae = ce.Expression as PropertyAccessExpression;
                    result.Type = GetExpandedExpression(pae);
                }

                foreach (var arg in ce.Arguments)
                {
                    switch (arg.Kind)
                    {
                        case SyntaxKind.StringLiteral:
                            result.Arguments.Add(new()
                            {
                                Value = (arg as StringLiteral).Text
                            });
                            break;

                        case SyntaxKind.NumericLiteral:
                            result.Arguments.Add(new()
                            {
                                Value = double.Parse((arg as LiteralExpression).Text, Invariants.NumberFormat)
                            });
                            break;

                        default:
                            result.Arguments.Add(new()
                            {
                                Value = null
                            });
                            break;
                    }
                }
            }
            else if (decorator.Expression.Kind == SyntaxKind.PropertyAccessExpression)
            {
                pae = decorator.Expression as PropertyAccessExpression;
                result.Type = GetExpandedExpression(pae);
            }

            return result;
        }

        static List<ExternalMember> GetClassMembers(ClassDeclaration node)
        {
            var result = new List<ExternalMember>();
            var used = new HashSet<string>();

            if (node.Members == null)
                return result;

            foreach (var member in node.Members) 
            {

                if (member.Kind != SyntaxKind.MethodDeclaration &&
                    member.Kind != SyntaxKind.PropertyDeclaration &&
                    member.Kind != SyntaxKind.Constructor)
                    continue;

                var name = member.Name != null ? member.Name.GetText() : "$ctor";

                if (!used.Add(name))
                    continue;

                ExternalMember externalMember;

                if (member.Kind == SyntaxKind.PropertyDeclaration)
                {
                    externalMember = new ExternalMember();

                    var pd = (member as PropertyDeclaration);
                    if (pd.Type != null)
                        externalMember.Type = GetExpandedExpression(pd.Type);
                }
                else if (member.Kind == SyntaxKind.MethodDeclaration)
                {
                    externalMember = new ExternalMethod
                    {
                        Arguments = new()
                    };
                    var md = (member as MethodDeclaration);
                    if (md.Type != null)
                        externalMember.Type = GetExpandedExpression(md.Type);

                    foreach (var arg in md.Parameters)
                    {
                        (externalMember as ExternalMethod).Arguments.Add(new()
                        {
                            Name = arg.Name.GetText(),
                            Type = GetExpandedExpression(arg.Type)
                        });
                    }
                }
                else if (member.Kind == SyntaxKind.Constructor)
                {
                    externalMember = new ExternalMethod
                    {
                        Arguments = new(),
                        IsConstructor = true
                    };
                    var md = member as ConstructorDeclaration;

                    foreach (var arg in md.Parameters)
                    {
                        (externalMember as ExternalMethod).Arguments.Add(new()
                        {
                            Name = arg.Name.GetText(),
                            Type = GetExpandedExpression(arg.Type)
                        });
                    }
                }
                else
                    continue;

                externalMember.Name = name;
                externalMember.IsStatic = member.Modifiers != null && member.Modifiers.Any(x => x.GetText() == "static");
                externalMember.Attributes = member.Decorators?.Select(DecoratorToExternalAttribute).ToList();

                result.Add(externalMember);
            }

            return result;
        }

        static List<ExternalMember> GetInterfaceMembers(InterfaceDeclaration node)
        {
            if (node.Members == null)
                return null;

            var result = new List<ExternalMember>();
            var used = new HashSet<string>();

            foreach (var member in node.Members) 
            {
                if (member.Kind != SyntaxKind.PropertySignature &&
                    member.Kind != SyntaxKind.MethodSignature)
                    continue;

                var name = member.Name.GetText();
                if (!used.Add(name))
                    continue;

                ExternalMember externalMember;

                if (member.Kind == SyntaxKind.PropertySignature)
                {
                    externalMember = new();
                    var pd = member as PropertySignature;
                    if (pd.Type != null)
                        externalMember.Type = GetExpandedExpression(pd.Type);
                }
                else if (member.Kind == SyntaxKind.MethodSignature)
                {
                    externalMember = new ExternalMethod
                    {
                        Arguments = new()
                    };
                    var md = member as MethodSignature;
                    if (md.Type != null)
                        externalMember.Type = GetExpandedExpression(md.Type);

                    foreach (var arg in md.Parameters)
                    {
                        (externalMember as ExternalMethod).Arguments.Add(new()
                        {
                            Name = arg.Name.GetText(),
                            Type = GetExpandedExpression(arg.Type)
                        });
                    }
                }
                else
                    continue;

                externalMember.Name = name;
                externalMember.Attributes = member.Decorators?.Select(DecoratorToExternalAttribute).ToList();

                result.Add(externalMember);
            }
            return result;
        }

        static ExternalType ClassToExternalType(ClassDeclaration klass)
        {
            var result = new ExternalType 
            {
                BaseType = GetBaseType(klass),
                GenericParameters = TypeParametersToExternal(klass.TypeParameters),
                IsAbstract = klass.Modifiers != null && klass.Modifiers.Any(x => x.Kind == SyntaxKind.AbstractKeyword) == true,
                IsSealed = false,
                IsSerializable = false,
                Namespace = GetNamespace(klass),
                Name = klass.Name.GetText(),
                IsInterface = false,
                IsDeclaration = IsUnderAmbientNamespace(klass)
            };

            var members = GetClassMembers(klass);
            result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
            result.Methods = members?.OfType<ExternalMethod>().ToList();
            result.Interfaces = GetInterfaces(klass);
            result.Attributes = klass.Decorators?.Select(DecoratorToExternalAttribute).ToList();

            return result;
        }

        static ExternalType InterfaceToExternalType(InterfaceDeclaration intf)
        {
            var result = new ExternalType 
            {
                GenericParameters = TypeParametersToExternal(intf.TypeParameters),
                Namespace = GetNamespace(intf),
                Name = intf.Name.GetText(),
                IsInterface = true,
                IsDeclaration = IsUnderAmbientNamespace(intf)
            };

            var members = GetInterfaceMembers(intf);
            result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
            result.Methods = members?.OfType<ExternalMethod>().ToList();
            result.Interfaces = GetBaseInterfaces(intf);
            result.Attributes = intf.Decorators?.Select(DecoratorToExternalAttribute).ToList();

            return result;
        }

        static List<ExternalMember> GetModuleMembers(ModuleDeclaration node)
        {
            var result = new List<ExternalMember>();
            var used = new HashSet<string>();

            foreach (var member in node.Body.Children) 
            {
                if (member.Kind != SyntaxKind.MethodDeclaration &&
                    member.Kind != SyntaxKind.PropertyDeclaration)
                    continue;

                var name = ((member as MethodDeclaration)?.Name ?? (member as PropertyDeclaration)?.Name)?.GetText();
                if (name == null || !used.Add(name))
                    continue;

                ExternalMember externalMember;

                if (member.Kind == SyntaxKind.PropertyDeclaration)
                {
                    externalMember = new();
                    var pd = member as PropertyDeclaration;
                    if (pd.Type != null)
                        externalMember.Type = GetExpandedExpression(pd.Type);
                }
                else if (member.Kind == SyntaxKind.MethodDeclaration)
                {
                    externalMember = new ExternalMethod
                    {
                        Arguments = new()
                    };
                    var md = member as MethodDeclaration;
                    if (md.Type != null)
                        externalMember.Type = GetExpandedExpression(md.Type);

                    foreach (var arg in md.Parameters)
                    {
                        (externalMember as ExternalMethod).Arguments.Add(new()
                        {
                            Name = arg.Name.GetText(),
                            Type = GetExpandedExpression(arg.Type)
                        });
                    }
                }
                else
                    continue;

                externalMember.Name = name;
                externalMember.IsStatic = true;
                externalMember.Attributes = member.Decorators?.Select(DecoratorToExternalAttribute).ToList();

                result.Add(externalMember);
            }

            return result;
        }

        static ExternalType ModuleToExternalType(ModuleDeclaration module) 
        {
            var result = new ExternalType 
            {
                Namespace = GetNamespace(module),
                Name = module.Name.GetText(),
                IsInterface = true,
                IsDeclaration = IsUnderAmbientNamespace(module)
            };

            var members = GetModuleMembers(module);
            result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
            result.Methods = members?.OfType<ExternalMethod>().ToList();

            return result;
        }

        private static List<ExternalType> ExtractTypes(SourceFile sourceFile) 
        {
            var result = new List<ExternalType>();

            INode visitNode(INode node)
            {
                if (node == null)
                    return null;

                switch (node.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        var klass = node as ClassDeclaration;
                        if (sourceFile.IsDeclarationFile || HasExportModifier(node)) 
                        {
                            var name = PrependNamespace(klass.Name.GetText(), klass);
                            var exportedType = ClassToExternalType(klass);
                            result.Add(exportedType);
                        }

                        return null;

                    case SyntaxKind.InterfaceDeclaration:
                        var intf = node as InterfaceDeclaration;

                        if (sourceFile.IsDeclarationFile || HasExportModifier(node)) 
                        {
                            var name = PrependNamespace(intf.Name.GetText(), intf);
                            var exportedType = InterfaceToExternalType(intf);
                            result.Add(exportedType);
                        }
                        return null;    

                    case SyntaxKind.ModuleDeclaration:
                        var modul = node as ModuleDeclaration;

                        if (sourceFile.IsDeclarationFile || HasExportModifier(modul) ||
                            (!IsUnderAmbientNamespace(modul) && !HasDeclareModifier(modul))) 
                        {
                            var name = PrependNamespace(modul.Name.GetText(), modul);
                            var exportedType = ModuleToExternalType(modul);
                            result.Add(exportedType);
                        }
                        break;
                }

                return Ts.ForEachChild(node, child => visitNode(child));
            }

            visitNode(sourceFile);

            return result;
        }

        public List<ExternalType> ExtractTypes()
        {
            var result = new List<ExternalType>();
            var visited = new HashSet<string>();
            foreach (var file in files)
            {
                if ((file.RootNode as SourceFile).FileName == "/lib.d.ts")
                    continue;

                //if (!((file.RootNode as SourceFile).FileName.Contains("serenity.corelib", StringComparison.OrdinalIgnoreCase)))
                //    continue;

                var types = ExtractTypes(file.RootNode as SourceFile);

                foreach (var k in types)
                {
                    var fullName = !string.IsNullOrEmpty(k.Namespace) ? k.Namespace + "." + k.Name : k.Name;
                    if (!visited.Add(fullName))
                        continue;

                    result.Add(k);
                }
            }

            return result;
        }
    }
}