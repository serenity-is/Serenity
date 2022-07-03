namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        public bool LocalTexts { get; set; }
        public readonly HashSet<string> LocalTextFilters = new();

#if ISSOURCEGENERATOR
        public ServerTypingsGenerator(Microsoft.CodeAnalysis.Compilation compilation, 
            System.Threading.CancellationToken cancellationToken)
            : base(compilation, cancellationToken)
        {
        }
#else
        public ServerTypingsGenerator(IGeneratorFileSystem fileSystem, params Assembly[] assemblies)
            : base(fileSystem, assemblies)
        {
        }

        public ServerTypingsGenerator(IGeneratorFileSystem fileSystem, params string[] assemblyLocations)
            : base(fileSystem, assemblyLocations)
        {
        }
#endif

        protected override void GenerateAll()
        {
            base.GenerateAll();
            if (LocalTexts)
                GenerateTexts();
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
            var codeNamespace = GetNamespace(type);

            void run(Action<TypeDefinition> action)
            {
                cw.Indented("namespace ");
                sb.Append(codeNamespace);
                cw.InBrace(delegate
                {
                    action(type);
                });
            }

            if (type.IsEnum())
                run(GenerateEnum);
            else if (TypingsUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "Controller") ||
                TypingsUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
                run(GenerateService);
            else
            {
                var formScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
                if (formScriptAttr != null)
                {
                    run(t => GenerateForm(t, formScriptAttr));
                    EnqueueTypeMembers(type);

                    if (TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest"))
                    {
                        AddFile(RemoveRootNamespace(codeNamespace,
                            fileIdentifier + ".ts"));

                        fileIdentifier = type.Name;
                        run(GenerateBasicType);
                    }

                    return;
                }

                var columnsScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute");
                if (columnsScriptAttr != null)
                {
                    run(t => GenerateColumns(t, columnsScriptAttr));
                    EnqueueTypeMembers(type);
                    return;
                }

                if (TypingsUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null ||
                    TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute") != null)
                {
                    run(GeneratePermissionKeys);
                }
                else
                    run(GenerateBasicType);
            }
        }
    }
}