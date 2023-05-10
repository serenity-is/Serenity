#if ISSOURCEGENERATOR
using System.Collections.Concurrent;
#else
using Serenity.CodeGeneration;
#endif
using Serenity.TypeScript;
using Serenity.TypeScript.TsTypes;
using System.Threading;

namespace Serenity.CodeGenerator;

public class TSTypeListerAST
{
    private readonly List<string> fileNames = new();
    private readonly HashSet<string> exportedTypeNames = new();
    private readonly IGeneratorFileSystem fileSystem;
    private readonly ConcurrentDictionary<string, SourceFile> astCache;
    private readonly TSModuleResolver moduleResolver;
    private readonly CancellationToken cancellationToken;

    public TSTypeListerAST(IGeneratorFileSystem fileSystem, string tsConfigDir,
        TSConfig tsConfig, ConcurrentDictionary<string, SourceFile> astCache = null, 
        CancellationToken cancellationToken = default)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.astCache = astCache;
        if (tsConfigDir is null || tsConfigDir is "")
            throw new ArgumentNullException(nameof(tsConfigDir));

        moduleResolver = new TSModuleResolver(fileSystem, tsConfigDir, tsConfig);
        this.cancellationToken = cancellationToken;
    }

    public void AddInputFile(string path)
    {
        fileNames.Add(path);
    }

    static bool HasExportModifier(INode node)
    {
        return node.Modifiers != null && node.Modifiers.Any(x => x.Kind == SyntaxKind.ExportKeyword);
    }

    static bool HasDeclareModifier(INode node)
    {
        return node.Modifiers != null && node.Modifiers.Any(x => x.Kind == SyntaxKind.DeclareKeyword);
    }

    static IEnumerable<INode> EnumerateParents(INode node)
    {
        if (node == null)
            yield break;

        while ((node = node.Parent) != null)
            yield return node;
    }

    static IEnumerable<INode> EnumerateTypesAndModules(IEnumerable<INode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Kind == SyntaxKind.ModuleDeclaration)
            {
                yield return node;

                var md = (node as ModuleDeclaration);
                while (md.Body is ModuleDeclaration smd)
                {
                    yield return smd;
                    md = smd;
                }

                if (md.Body is Block b)
                {
                    if (b.Statements != null)
                        foreach (var x in EnumerateTypesAndModules(b.Statements))
                            yield return x;
                }
            }
            else if (node.Kind == SyntaxKind.ClassDeclaration ||
                node.Kind == SyntaxKind.InterfaceDeclaration ||
                node.Kind == SyntaxKind.EnumDeclaration)
                yield return node;
        }
    }

    static string GetNamespace(INode node)
    {
        return string.Join(".", EnumerateParents(node)
            .Where(x => x.Kind == SyntaxKind.ModuleDeclaration)
            .Select(x => GetText((x as ModuleDeclaration).Name))
            .Reverse());
    }

    static string PrependNamespace(string s, INode node)
    {
        var ns = GetNamespace(node);
        if (!string.IsNullOrEmpty(ns))
            return ns + "." + s;
        return s;
    }

    static string AddTypeArgs(string text, NodeArray<ITypeNode> args)
    {
        if (args != null &&
            args.Count > 0)
        {
            if (args.Count == 1)
                return text + "<" + GetText(args[0]) + ">";
            else
                return text + "<" + string.Join(",", args.Select(x => GetText(x)) + ">");
        }

        return text;
    }

    static string GetText(INode node)
    {
        if (node is PropertyAccessExpression pac)
        {
            return GetText(pac.Expression) + "." + GetText(pac.Name);
        }
        else if (node is QualifiedName qfn)
        {
            return GetText(qfn.Left) + "." + GetText(qfn.Right);
        }
        else if (node is ExpressionWithTypeArguments ewta)
        {
            if (ewta.Expression is Identifier ewti && ewti.Text != null)
                return AddTypeArgs(ewti.Text, ewta.TypeArguments);
            if (ewta.Expression is PropertyAccessExpression ewpa)
                return AddTypeArgs(GetText(ewpa), ewta.TypeArguments);
        }
        else if (node is Identifier idt && idt.Text is not null)
            return idt.Text;
        else if (node is TypeNode)
        {
            if (node.Kind == SyntaxKind.StringKeyword)
                return "string";
            else if (node.Kind == SyntaxKind.AnyKeyword)
                return "any";
            else if (node.Kind == SyntaxKind.BooleanKeyword)
                return "boolean";
            else if (node.Kind == SyntaxKind.NumberKeyword)
                return "number";
            if (node is TypeReferenceNode tr)
            {
                if (tr.TypeName is Identifier tri && tri.Text != null)
                    return AddTypeArgs(tri.Text, tr.TypeArguments);

                if (tr.TypeName is QualifiedName trq)
                    return AddTypeArgs(GetText(trq), tr.TypeArguments);
            }

            if (node is ArrayTypeNode atn)
                return GetText(atn.ElementType) + "[]";
        }
        else if (node is Declaration de && de.Name != null)
            return GetText(de.Name);
        return node?.GetText();
    }

    string GetTypeReferenceExpression(INode node, bool isDecorator = false)
    {
        if (node == null)
            return string.Empty;

        var text = GetText(node);
        if (text is null || text.Length == 0 || text == "any")
            return null;

        if (text[0] == '(' || 
            text[0] == '{' ||
            text.Contains('|'))
            return null;


        if (text == "number" || text == "string" || text == "boolean")
            return text;

        var noGeneric = text;
        var lt = noGeneric.IndexOf('<');
        if (lt >= 0 && noGeneric[^1] == '>')
            noGeneric = noGeneric[..lt];

        string functionSuffix = string.Empty;
        if (isDecorator)
        {
            var ldi = noGeneric.LastIndexOf('.');
            if (ldi > 0)
            {
                functionSuffix = noGeneric[ldi..].ToString();
                noGeneric = noGeneric[..ldi];
            }
        }

        var dotIndex = noGeneric.IndexOf('.');
        var beforeDot = dotIndex >= 0 ? noGeneric[..dotIndex] : null;
        var afterDot = dotIndex >= 0 ? noGeneric[dotIndex..] : null;

        var parents = EnumerateParents(node).ToArray();

        foreach (var parent in parents)
        {
            if (parent.Kind == SyntaxKind.ModuleDeclaration ||
                parent.Kind == SyntaxKind.SourceFile)
            {
                IEnumerable<INode> children;
                if (parent is ModuleDeclaration md)
                    children = (md.Body as ModuleBlock)?.Statements;
                else
                    children = ((SourceFile)parent).Statements;

                if (children == null)
                    continue;

                if (parent is SourceFile sourceFile &&
                    sourceFile.ExternalModuleIndicator is not null)
                {
                    foreach (var import in children.OfType<ImportDeclaration>())
                    {
                        if (import.ModuleSpecifier is not StringLiteral id)
                            continue;

                        if (import.ImportClause?.Name is Identifier name &&
                            name.Text == noGeneric)
                            return id.Text + ":" + name.Text + functionSuffix;

                        var elements = (import.ImportClause?.NamedBindings as NamedImports)?.Elements;
                        if (elements is null)
                            continue;

                        foreach (var element in elements)
                        {
                            if (element.Name is Identifier nameIdentifier &&
                                nameIdentifier.Text == noGeneric)
                            {
                                var module = id.Text;
                                if (module.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                                    module = ResolveModule(id.Text, sourceFile.FileName, false).moduleName ?? module;

                                return module + ":" + nameIdentifier.Text + functionSuffix;
                            }
                        }
                    }
                }

                if (dotIndex < 0)
                {
                    foreach (var child in children)
                    {
                        if ((child.Kind == SyntaxKind.ClassDeclaration &&
                             GetText((child as ClassDeclaration).Name) == noGeneric) ||
                            (child.Kind == SyntaxKind.InterfaceDeclaration &&
                             GetText((child as InterfaceDeclaration).Name) == noGeneric) ||
                            (child.Kind == SyntaxKind.EnumDeclaration && 
                             GetText((child as EnumDeclaration).Name) == noGeneric))
                            return PrependNamespace(noGeneric.ToString(), child) + functionSuffix;
                    }
                }
                else
                {
                    foreach (var child in children)
                    {
                        if (child.Kind == SyntaxKind.ImportEqualsDeclaration)
                        {
                            if (GetText((child as ImportEqualsDeclaration).Name) == beforeDot)
                            {
                                var fullName = GetText((child as ImportEqualsDeclaration).ModuleReference) +
                                    afterDot.ToString();
                                if (exportedTypeNames.Contains(fullName))
                                    return fullName + functionSuffix;
                            }
                        }
                    }
                }
            }
        }

        var ns = GetNamespace(node);
        while (!string.IsNullOrEmpty(ns))
        {
            var s = ns + "." + noGeneric.ToString();
            if (exportedTypeNames.Contains(s))
                return s + functionSuffix;

            var idx = ns.LastIndexOf('.');
            if (idx >= 0)
                ns = ns[..idx];
            else 
                break;
        }

        return noGeneric.ToString() + functionSuffix;
    }

    string GetBaseType(ClassDeclaration node)
    {
        if (node.HeritageClauses == null)
            return null;

        foreach (var heritage in node.HeritageClauses) 
        {
            if (heritage.Token == SyntaxKind.ExtendsKeyword &&
                heritage.Types != null) 
            {
                foreach (var type in heritage.Types) 
                    return GetTypeReferenceExpression(type);
            }
        }

        return null;
    }

    List<string> GetInterfaces(ClassDeclaration node)
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
                    result.Add(GetTypeReferenceExpression(type));
            }
        }

        return result.Any() ? result : null;
    }

    List<string> GetBaseInterfaces(InterfaceDeclaration node)
    {
        if (node.HeritageClauses == null)
            return null;

        var result = new List<string>();
        foreach (var heritage in node.HeritageClauses)
        {
            if (heritage.Token == SyntaxKind.ExtendsKeyword &&
                heritage.Types != null)
            {
                foreach (var type in heritage.Types)
                    result.Add(GetTypeReferenceExpression(type));
            }
        }

        return result.Count == 0 ? null : result;
    }

    static List<ExternalGenericParameter> TypeParametersToExternal(NodeArray<TypeParameterDeclaration> p) 
    {
        if (p == null || p.Count == 0)
            return null;

        return p.Select(k => new ExternalGenericParameter
        {
            Name = GetText(k)
        }).ToList();
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

    ExternalAttribute DecoratorToExternalAttribute(Decorator decorator)
    {
        if (decorator.Expression == null)
            return null;

        var result = new ExternalAttribute();

        PropertyAccessExpression pae;
        if (decorator.Expression.Kind == SyntaxKind.CallExpression)
        {
            var ce = decorator.Expression as CallExpression;
            if (ce.Expression != null &&
                ce.Expression.Kind == SyntaxKind.PropertyAccessExpression)
            {
                pae = ce.Expression as PropertyAccessExpression;
                result.Type = GetTypeReferenceExpression(pae, isDecorator: true);
            }

            if (ce.Arguments != null &&
                ce.Arguments.Count > 0)
                result.Arguments ??= new();

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
                            Value = double.Parse((arg as LiteralExpression).Text, CultureInfo.InvariantCulture.NumberFormat)
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
            result.Type = GetTypeReferenceExpression(pae);
        }

        return result;
    }

    List<ExternalMember> GetClassMembers(ClassDeclaration node)
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

            var name = member.Name != null ? GetText(member.Name) : "$ctor";

            if (!used.Add(name))
                continue;

            ExternalMember externalMember;

            if (member.Kind == SyntaxKind.PropertyDeclaration)
            {
                externalMember = new ExternalMember();

                var pd = (member as PropertyDeclaration);
                if (pd.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(pd.Type);
                if (pd.Initializer is StringLiteral sl)
                    externalMember.Value = sl.Text;
                else if (pd.Initializer?.Kind == SyntaxKind.FalseKeyword)
                    externalMember.Value = false;
                else if (pd.Initializer?.Kind == SyntaxKind.TrueKeyword)
                    externalMember.Value = true;
            }
            else if (member.Kind == SyntaxKind.MethodDeclaration)
            {
                externalMember = new ExternalMethod
                {
                    Arguments = new()
                };
                var md = (member as MethodDeclaration);
                if (md.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(md.Type);

                foreach (var arg in md.Parameters)
                {
                    (externalMember as ExternalMethod).Arguments.Add(new()
                    {
                        Name = GetText(arg.Name),
                        Type = GetTypeReferenceExpression(arg.Type)
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
                        Name = GetText(arg.Name),
                        Type = GetTypeReferenceExpression(arg.Type)
                    });
                }
            }
            else
                continue;

            externalMember.Name = name;
            if (member.Modifiers != null && member.Modifiers.Any(x => x.Kind == SyntaxKind.StaticKeyword))
                externalMember.IsStatic = true;
            if (member.Decorators != null && member.Decorators.Count > 0)
                externalMember.Attributes = member.Decorators.Select(DecoratorToExternalAttribute).ToList();

            result.Add(externalMember);
        }

        return result;
    }

    List<ExternalMember> GetInterfaceMembers(InterfaceDeclaration node)
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

            var name = (member.Name as ILiteralLikeNode)?.Text ??
                (member.Name as Identifier).Text;
            if (!used.Add(name))
                continue;

            ExternalMember externalMember;

            if (member.Kind == SyntaxKind.PropertySignature)
            {
                externalMember = new();
                var pd = member as PropertySignature;
                if (pd.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(pd.Type);
            }
            else if (member.Kind == SyntaxKind.MethodSignature)
            {
                externalMember = new ExternalMethod
                {
                    Arguments = new()
                };
                var md = member as MethodSignature;
                if (md.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(md.Type);

                foreach (var arg in md.Parameters)
                {
                    (externalMember as ExternalMethod).Arguments.Add(new()
                    {
                        Name = (arg.Name as ILiteralLikeNode)?.Text ??
                            (arg.Name as Identifier).Text,
                        Type = GetTypeReferenceExpression(arg.Type)
                    });
                }
            }
            else
                continue;

            externalMember.Name = name;
            if (member.Decorators != null && member.Decorators.Count > 0)
                externalMember.Attributes = member.Decorators.Select(DecoratorToExternalAttribute).ToList();

            result.Add(externalMember);
        }
        return result;
    }

    ExternalType EnumToExternalType(EnumDeclaration enumDec)
    {
        var result = new ExternalType
        {
            Namespace = GetNamespace(enumDec),
            Name = GetText(enumDec.Name),
            IsDeclaration = IsUnderAmbientNamespace(enumDec) ? true : null
        };

        return result;
    }

    ExternalType ClassToExternalType(ClassDeclaration klass)
    {
        var result = new ExternalType 
        {
            BaseType = GetBaseType(klass),
            GenericParameters = TypeParametersToExternal(klass.TypeParameters),
            IsAbstract = klass.Modifiers != null && klass.Modifiers.Any(x => x.Kind == SyntaxKind.AbstractKeyword) == true ? true : null,
            Namespace = GetNamespace(klass),
            Name = GetText(klass.Name),
            IsDeclaration = IsUnderAmbientNamespace(klass) ? true : null
        };

        var members = GetClassMembers(klass);
        result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
        if (result.Fields != null && result.Fields.Count == 0)
            result.Fields = null;
        result.Methods = members.OfType<ExternalMethod>().ToList();
        if (result.Methods != null && result.Methods.Count == 0)
            result.Methods = null;
        result.Interfaces = GetInterfaces(klass);
        if (klass.Decorators?.Any() == true)
            result.Attributes = klass.Decorators.Select(DecoratorToExternalAttribute).ToList();

        return result;
    }

    ExternalType InterfaceToExternalType(InterfaceDeclaration intf)
    {
        var result = new ExternalType 
        {
            GenericParameters = TypeParametersToExternal(intf.TypeParameters),
            Namespace = GetNamespace(intf),
            Name = GetText(intf.Name),
            IsInterface = true,
            IsDeclaration = IsUnderAmbientNamespace(intf) ? true : null
        };

        var members = GetInterfaceMembers(intf);
        result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
        if (result.Fields != null && result.Fields.Count == 0)
            result.Fields = null;
        result.Methods = members?.OfType<ExternalMethod>().ToList();
        if (result.Methods != null && result.Methods.Count == 0)
            result.Methods = null;
        result.Interfaces = GetBaseInterfaces(intf);
        if (intf.Decorators?.Any() == true)
            result.Attributes = intf.Decorators.Select(DecoratorToExternalAttribute).ToList();

        return result;
    }

    List<ExternalMember> GetModuleMembers(ModuleDeclaration node)
    {
        var result = new List<ExternalMember>();
        var used = new HashSet<string>();

        var statements = (node.Body as Block)?.Statements;
        if (statements is null)
            return result;

        foreach (var member in statements) 
        {
            if (member.Kind != SyntaxKind.MethodDeclaration &&
                member.Kind != SyntaxKind.PropertyDeclaration)
                continue;

            var name = GetText((member as MethodDeclaration)?.Name ?? (member as PropertyDeclaration)?.Name);
            if (name == null || !used.Add(name))
                continue;

            ExternalMember externalMember;

            if (member.Kind == SyntaxKind.PropertyDeclaration)
            {
                externalMember = new();
                var pd = member as PropertyDeclaration;
                if (pd.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(pd.Type);
            }
            else if (member.Kind == SyntaxKind.MethodDeclaration)
            {
                externalMember = new ExternalMethod
                {
                    Arguments = new()
                };
                var md = member as MethodDeclaration;
                if (md.Type != null)
                    externalMember.Type = GetTypeReferenceExpression(md.Type);

                foreach (var arg in md.Parameters)
                {
                    (externalMember as ExternalMethod).Arguments.Add(new()
                    {
                        Name = GetText(arg.Name),
                        Type = GetTypeReferenceExpression(arg.Type)
                    });
                }
            }
            else
                continue;

            externalMember.Name = name;
            externalMember.IsStatic = true;
            if (member.Decorators != null && member.Decorators.Count > 0)
                externalMember.Attributes = member.Decorators?.Select(DecoratorToExternalAttribute).ToList();

            result.Add(externalMember);
        }

        return result;
    }

    ExternalType ModuleToExternalType(ModuleDeclaration module) 
    {
        var result = new ExternalType 
        {
            Namespace = GetNamespace(module),
            Name = GetText(module.Name),
            IsInterface = true,
            IsDeclaration = IsUnderAmbientNamespace(module) ? true : null
        };

        var members = GetModuleMembers(module);
        result.Fields = members?.Where(x => x is not ExternalMethod).ToList();
        if (result.Fields != null && result.Fields.Count == 0)
            result.Fields = null;
        result.Methods = members?.OfType<ExternalMethod>().ToList();
        if (result.Methods != null && result.Methods.Count == 0)
            result.Methods = null;

        return result;
    }

    List<ExternalType> ExtractTypes(SourceFile sourceFile, HashSet<string> visited)
    {
        var result = new List<ExternalType>();

        if (sourceFile.Statements == null)
            return result;

        if (!sourceFileInfos.TryGetValue(sourceFile, out var sourceFileInfo))
            sourceFileInfo = null;

        if (sourceFile.ExternalModuleIndicator is not null)
        {
            foreach (var exportDeclaration in sourceFile
                .Statements.OfType<ExportDeclaration>())
            {
                string identifier;
                if (exportDeclaration.ModuleSpecifier is not Identifier ms)
                {
                    if (exportDeclaration.ModuleSpecifier is not StringLiteral sl)
                        continue;
                    identifier = sl.Text;
                }
                else
                    identifier = ms.Text;

                if (string.IsNullOrEmpty(identifier))
                    continue;

                var resolvedSub = ResolveModule(identifier, sourceFile.FileName, false);
                if (resolvedSub.fullPath is null ||
                    visited.Contains(resolvedSub.fullPath))
                    continue;

                visited.Add(resolvedSub.fullPath);
                var subFile = ParseFile(resolvedSub.fullPath, sourceFileInfo?.ModuleName, true);
                if (subFile is null)
                    continue;

                var subTypes = ExtractTypes(subFile, visited);

                void addSubType(ExternalType type)
                {
                    type.Module = sourceFileInfo.ModuleName;
                    type.Namespace = null;

                    if (sourceFileInfo?.ModuleName != null &&
                        !sourceFileInfo.ModuleName.StartsWith("/") &&
                        !sourceFileInfo.ModuleName.StartsWith("."))
                    {
                        if (type.BaseType != null &&
                            type.BaseType.Contains(':') &&
                            type.BaseType.StartsWith("."))
                            type.BaseType = sourceFileInfo.ModuleName + ":" + type.BaseType.Split(':').Last();

                        if (type.Attributes != null)
                        {
                            foreach (var attr in type.Attributes)
                            {
                                if (attr.Type != null &&
                                    attr.Type.Contains(":", StringComparison.Ordinal) &&
                                    attr.Type.StartsWith(".", StringComparison.Ordinal))
                                    attr.Type = sourceFileInfo.ModuleName + ":" + attr.Type.Split(':').Last();
                            }
                        }
                    }

                    result.Add(type);
                }

                if (exportDeclaration.ExportClause is not NamedExports namedExports)
                {
                    foreach (var type in subTypes.Where(x => string.IsNullOrEmpty(x.Namespace)))
                    {
                        addSubType(type);
                    }
                }
                else if (namedExports.Elements is not null)
                {
                    foreach (var element in namedExports.Elements)
                    {
                        if (element.Name is Identifier nameIdentifier)
                        {
                            foreach (var type in subTypes.Where(x => string.IsNullOrEmpty(x.Namespace) &&
                                x.Name == (element.PropertyName?.Text ?? nameIdentifier.Text)))
                            {
                                addSubType(type);
                            }
                        }
                    }
                }
            }
        }

        foreach (var node in EnumerateTypesAndModules(sourceFile.Statements))
        {
            switch (node.Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    var klass = node as ClassDeclaration;
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node)) 
                    {
                        var exportedType = ClassToExternalType(klass);
                        result.Add(exportedType);
                    }
                    break;

                case SyntaxKind.EnumDeclaration:
                    var enumDec = node as EnumDeclaration;
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                    {
                        var exportedType = EnumToExternalType(enumDec);
                        result.Add(exportedType);
                    }
                    break;


                case SyntaxKind.InterfaceDeclaration:
                    var intf = node as InterfaceDeclaration;

                    if (sourceFile.IsDeclarationFile || HasExportModifier(node)) 
                    {
                        var exportedType = InterfaceToExternalType(intf);
                        result.Add(exportedType);
                    }
                    break;

                case SyntaxKind.ModuleDeclaration:
                    var modul = node as ModuleDeclaration;

                    if (sourceFile.IsDeclarationFile || HasExportModifier(modul) ||
                        (!IsUnderAmbientNamespace(modul) && !HasDeclareModifier(modul))) 
                    {
                        var exportedType = ModuleToExternalType(modul);
                        result.Add(exportedType);
                    }
                    break;
            }
        }

        return result;
    }

    private static void ExtractExportedTypeNames(SourceFile sourceFile, HashSet<string> target)
    {
        if (sourceFile?.Statements == null ||
            sourceFile?.ExternalModuleIndicator is not null)
            return;

        foreach (var node in EnumerateTypesAndModules(sourceFile.Statements))
        {
            switch (node.Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                    {
                        var klass = node as ClassDeclaration;
                        target.Add(PrependNamespace(GetText(klass.Name), klass));
                    }
                    break;

                case SyntaxKind.EnumDeclaration:
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                    {
                        var enumDec = node as EnumDeclaration;
                        target.Add(PrependNamespace(GetText(enumDec.Name), enumDec));
                    }
                    break;

                case SyntaxKind.InterfaceDeclaration:
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                    {
                        var intf = node as InterfaceDeclaration;
                        target.Add(PrependNamespace(GetText(intf.Name), intf));
                    }
                    break;

                case SyntaxKind.ModuleDeclaration:
                    if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                    {
                        var modul = node as ModuleDeclaration;
                        if (sourceFile.IsDeclarationFile || HasExportModifier(modul) ||
                            (!IsUnderAmbientNamespace(modul) && !HasDeclareModifier(modul)))
                            target.Add(PrependNamespace(GetText(modul.Name), modul));
                    }
                    break;
            }
        }
    }

    readonly ConcurrentQueue<(string fullPath, string moduleName)> processFileQueue = new();
    readonly ConcurrentDictionary<string, Lazy<string>> processByFullPath = new();
    readonly ConcurrentDictionary<string, (string fullPath, string moduleName)> resolvedExternals = new();
    readonly ConcurrentDictionary<string, SourceFile> parsedFiles = new();

    private class SourceFileInfo
    {
        public string ModuleName { get; set; }
        public Dictionary<string, ResolvedModuleFull> ResolvedModules { get; set; }
    }

    private readonly ConcurrentDictionary<SourceFile, SourceFileInfo> sourceFileInfos = new();

    SourceFile ParseFile(string fileFullPath, string moduleName, bool extractExportsOnly)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return parsedFiles.GetOrAdd(fileFullPath, (fileFullPath) =>
        {
            var sourceFileText = fileSystem.ReadAllText(fileFullPath);

            SourceFile parseSourceFile()
            {
                return (SourceFile)new TypeScriptAST(sourceFileText,
                    fileFullPath, optimized: true).RootNode;
            }

            var sourceFile = astCache?.GetOrAdd(sourceFileText, _ => parseSourceFile()) ?? parseSourceFile();

            if (sourceFile.ExternalModuleIndicator is not null)
            {
                var sourceFileInfo = sourceFileInfos.GetOrAdd(sourceFile, static (s) => new SourceFileInfo());
                sourceFileInfo.ModuleName = moduleName;
                sourceFileInfo.ResolvedModules ??= new();

                foreach (var node in sourceFile.Statements)
                {
                    if (node.Kind == SyntaxKind.ImportEqualsDeclaration &&
                        node is ImportEqualsDeclaration ied &&
                        ied.ModuleReference?.Kind == SyntaxKind.ExternalModuleReference)
                    {
                    }
                    else if (node.Kind == SyntaxKind.ImportDeclaration &&
                        node is ImportDeclaration id &&
                        id.ModuleSpecifier is StringLiteral sl)
                    {
                        var resolved = ResolveModule(sl.Text, fileFullPath,
                            enqueue: !extractExportsOnly);

                        if (!string.IsNullOrEmpty(resolved.fullPath) &&
                            !sourceFileInfo.ResolvedModules.ContainsKey(sl.Text))
                        {
                            sourceFileInfo.ResolvedModules[sl.Text] = new ResolvedModuleFull
                            {
                                ResolvedFileName = resolved.fullPath,
                                IsExternalLibraryImport = !resolved.moduleName.StartsWith("/",
                                    StringComparison.Ordinal)
                            };
                        }
                    }
                }
            }

            return sourceFile;
        });
    }

    (string moduleName, string fullPath) ResolveModule(string fileNameOrModule, string referencedFrom, bool enqueue)
    {
        string resolvedModuleName;
        string resolvedFullPath;

        bool nonRelative = fileNameOrModule is not null &&
            fileNameOrModule.Length > 0 &&
            fileNameOrModule[0] != '.';

        if (nonRelative &&
            resolvedExternals.TryGetValue(fileNameOrModule, out var resolved))
        {
            resolvedModuleName = resolved.moduleName;
            resolvedFullPath = resolved.fullPath;
        }
        else
        {
            resolvedFullPath = moduleResolver.Resolve(fileNameOrModule, referencedFrom,
                out resolvedModuleName);

            if (resolvedFullPath is not null && nonRelative)
                resolvedExternals.TryAdd(fileNameOrModule, (resolvedFullPath, resolvedModuleName));
        }

        if (resolvedFullPath is null)
            return (null, null);

        if (enqueue)
        {
            return (processByFullPath.GetOrAdd(resolvedFullPath, x => new Lazy<string>(() =>
            {
                processFileQueue.Enqueue((resolvedFullPath, resolvedModuleName));
                return resolvedModuleName;
            })).Value, resolvedFullPath);
        }
        else
            return (resolvedModuleName, resolvedFullPath);
    }

    public List<ExternalType> ExtractTypes()
    {
        fileNames.AsParallel()
            .Select(x => ResolveModule(x, referencedFrom: null, enqueue: true))
            .ToArray();

        SourceFile[] sourceFiles = Array.Empty<SourceFile>();
        while (!processFileQueue.IsEmpty)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var currentQueue = processFileQueue.ToArray();
            while (!processFileQueue.IsEmpty)
                processFileQueue.TryDequeue(out _);
            sourceFiles = sourceFiles.Concat(currentQueue.AsParallel()
                .Select(x => ParseFile(x.fullPath, x.moduleName, extractExportsOnly: false)))
                .ToArray();
        }

        exportedTypeNames.Clear();
        foreach (var hashset in sourceFiles.AsParallel().Select(sourceFile =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var hashset = new HashSet<string>();
            ExtractExportedTypeNames(sourceFile, hashset);
            return hashset;
        }).ToArray())
        {
            foreach (var x in hashset)
                exportedTypeNames.Add(x);
        }

        var result = new List<ExternalType>();
        var resultIndex = new Dictionary<string, int>();

        foreach (var types in sourceFiles.AsParallel().Select(sourceFile =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = ExtractTypes(sourceFile, new HashSet<string>());
            foreach (var r in result)
            {
                r.SourceFile = sourceFile.FileName;
                r.Module = sourceFileInfos.TryGetValue(sourceFile, out var sfi) ? sfi.ModuleName : null;
            }
            return result;
        }).ToArray())
        {
            foreach (var k in types)
            {
                if (resultIndex.TryGetValue(k.FullName, out int index))
                {
                    continue;
                }
                else
                {
                    resultIndex[k.FullName] = result.Count;
                    result.Add(k);
                }
            }
        }

        return result;
    }
}