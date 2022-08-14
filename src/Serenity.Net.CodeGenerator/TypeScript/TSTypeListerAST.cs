#if ISSOURCEGENERATOR
using System.Collections.Concurrent;
#else
using Serenity;
using Serenity.CodeGeneration;
#endif
using Serenity.TypeScript;
using Serenity.TypeScript.TsTypes;
using System.Threading;

namespace Serenity.CodeGenerator
{
    public class TSTypeListerAST
    {
        private readonly List<string> fileNames = new();
        private readonly HashSet<string> exportedTypeNames = new();
        private readonly IGeneratorFileSystem fileSystem;
        private readonly TSModuleResolver moduleResolver;
        private readonly CancellationToken cancellationToken;

        public TSTypeListerAST(IGeneratorFileSystem fileSystem, string tsConfigDir,
            TSConfig tsConfig, CancellationToken cancellationToken = default)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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
                    node.Kind == SyntaxKind.InterfaceDeclaration)
                    yield return node;
            }
        }

        static string GetNamespace(INode node)
        {
            return string.Join(".", EnumerateParents(node)
                .Where(x => x.Kind == SyntaxKind.ModuleDeclaration)
                .Select(x => (x as ModuleDeclaration).Name.GetText())
                .Reverse());
        }

        static string PrependNamespace(string s, INode node)
        {
            var ns = GetNamespace(node);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + s;
            return s;
        }

        string GetTypeReferenceExpression(INode node, bool isDecorator = false)
        {
            if (node == null)
                return string.Empty;

            var text = node.GetTextSpan();
            if (text == "any" || text == null || text.Length == 0)
                return null;

            if (text[0] == '(' || 
                text[0] == '{' ||
                text.Contains('|'))
                return null;

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

            foreach (var parent in EnumerateParents(node))
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

                    if (dotIndex < 0)
                    {
                        foreach (var child in children)
                        {
                            if ((child.Kind == SyntaxKind.ClassDeclaration &&
                                 (child as ClassDeclaration).Name.GetTextSpan() == noGeneric) ||
                                (child.Kind == SyntaxKind.InterfaceDeclaration &&
                                 (child as InterfaceDeclaration).Name.GetTextSpan() == noGeneric))
                                return PrependNamespace(noGeneric.ToString(), child) + functionSuffix;
                        }
                    }
                    else
                    {
                        foreach (var child in children)
                        {
                            if (child.Kind == SyntaxKind.ImportEqualsDeclaration)
                            {
                                if ((child as ImportEqualsDeclaration).Name.GetTextSpan() == beforeDot)
                                {
                                    var fullName = (child as ImportEqualsDeclaration).ModuleReference.GetText() +
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
                Name = k.GetText()
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

                var name = member.Name != null ? member.Name.GetText() : "$ctor";

                if (!used.Add(name))
                    continue;

                ExternalMember externalMember;

                if (member.Kind == SyntaxKind.PropertyDeclaration)
                {
                    externalMember = new ExternalMember();

                    var pd = (member as PropertyDeclaration);
                    if (pd.Type != null)
                        externalMember.Type = GetTypeReferenceExpression(pd.Type);
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
                            Name = arg.Name.GetText(),
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
                            Name = arg.Name.GetText(),
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

        ExternalType ClassToExternalType(ClassDeclaration klass)
        {
            var result = new ExternalType 
            {
                BaseType = GetBaseType(klass),
                GenericParameters = TypeParametersToExternal(klass.TypeParameters),
                IsAbstract = klass.Modifiers != null && klass.Modifiers.Any(x => x.Kind == SyntaxKind.AbstractKeyword) == true ? true : null,
                Namespace = GetNamespace(klass),
                Name = klass.Name.GetText(),
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
                Name = intf.Name.GetText(),
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

                var name = ((member as MethodDeclaration)?.Name ?? (member as PropertyDeclaration)?.Name)?.GetText();
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
                            Name = arg.Name.GetText(),
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
                Name = module.Name.GetText(),
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

        List<ExternalType> ExtractTypes(SourceFile sourceFile) 
        {
            var result = new List<ExternalType>();

            if (sourceFile.Statements == null)
                return result;

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
            if (sourceFile?.Statements == null)
                return;

            foreach (var node in EnumerateTypesAndModules(sourceFile.Statements))
            {
                switch (node.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                        {
                            var klass = node as ClassDeclaration;
                            target.Add(PrependNamespace(klass.Name.GetText(), klass));
                        }
                        break;

                    case SyntaxKind.InterfaceDeclaration:
                        if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                        {
                            var intf = node as InterfaceDeclaration;
                            target.Add(PrependNamespace(intf.Name.GetText(), intf));
                        }
                        break;

                    case SyntaxKind.ModuleDeclaration:
                        if (sourceFile.IsDeclarationFile || HasExportModifier(node))
                        {
                            var modul = node as ModuleDeclaration;
                            if (sourceFile.IsDeclarationFile || HasExportModifier(modul) ||
                                (!IsUnderAmbientNamespace(modul) && !HasDeclareModifier(modul)))
                                target.Add(PrependNamespace(modul.Name.GetText(), modul));
                        }
                        break;
                }
            }
        }

        public List<ExternalType> ExtractTypes()
        {
            ConcurrentQueue<(string fullPath, string moduleName)> processFileQueue = new();
            ConcurrentDictionary<string, Lazy<string>> processByFullPath = new();
            ConcurrentDictionary<string, (string fullPath, string moduleName)> resolvedExternals = new();

            SourceFile parseFile(string fileFullPath, string moduleName)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sourceFile = (SourceFile)new TypeScriptAST(
                    fileSystem.ReadAllText(fileFullPath), 
                    fileFullPath, optimized: true).RootNode;

                if (sourceFile.ExternalModuleIndicator is not null)
                {
                    sourceFile.ModuleName = moduleName;
                    sourceFile.ResolvedModules ??= new();

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
                            var resolvedModuleName = resolveModule(sl.Text, fileFullPath);
                            if (!string.IsNullOrEmpty(resolvedModuleName) &&
                                !sourceFile.ResolvedModules.ContainsKey(sl.Text))
                            {
                                sourceFile.ResolvedModules[sl.Text] = new ResolvedModuleFull
                                {
                                    ResolvedFileName = resolvedModuleName,
                                    IsExternalLibraryImport = !resolvedModuleName.StartsWith("/", 
                                        StringComparison.Ordinal)
                                };
                            }
                        }
                    }
                }

                return sourceFile;
            }

            string resolveModule(string fileNameOrModule, string referencedFrom)
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
                    return null;

                return processByFullPath.GetOrAdd(resolvedFullPath, x => new Lazy<string>(() =>
                {
                    processFileQueue.Enqueue((resolvedFullPath, resolvedModuleName));
                    return resolvedModuleName;
                })).Value;
            }

            fileNames.AsParallel()
                .Select(x => resolveModule(x, referencedFrom: null))
                .ToArray();

            SourceFile[] sourceFiles = Array.Empty<SourceFile>();
            while (!processFileQueue.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var currentQueue = processFileQueue.ToArray();
                while (!processFileQueue.IsEmpty)
                    processFileQueue.TryDequeue(out _);
                sourceFiles = sourceFiles.Concat(currentQueue.AsParallel()
                    .Select(x => parseFile(x.fullPath, x.moduleName)))
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
                var result = ExtractTypes(sourceFile);
                foreach (var r in result)
                {
                    r.SourceFile = sourceFile.FileName;
                    r.Module = sourceFile.ModuleName;
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
}