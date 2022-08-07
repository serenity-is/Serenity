namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        public bool LocalTexts { get; set; }
        public bool NamespaceImports { get; set; }
        public bool NamespaceTypings { get; set; } = true;
        public bool ModuleTypings { get; set; } = false;

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
            if (NamespaceImports)
                GenerateNamespaceImports();
        }

        protected override void GenerateCodeFor(TypeDefinition type)
        {
            void add(Action<TypeDefinition, bool> action, string fileIdentifier = null)
            {
                if (NamespaceTypings)
                {
                    cw.Indented("namespace ");
                    var codeNamespace = GetNamespace(type);
                    sb.Append(codeNamespace);
                    cw.InBrace(delegate
                    {
                        action(type, false);
                    });
                    AddFile(RemoveRootNamespace(codeNamespace, (fileIdentifier ?? type.Name) + ".ts"), false);
                }

                if (ModuleTypings)
                {
                }
            }

            if (type.IsEnum())
            {
                add(GenerateEnum);
                return;
            }

            if (TypingsUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "Controller") ||
                TypingsUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
            {
                var controllerIdentifier = GetControllerIdentifier(type);
                add((t, m) => GenerateService(t, controllerIdentifier, m), controllerIdentifier);
                return;
            }

            var formScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
            if (formScriptAttr != null)
            {
                var formIdentifier = type.Name;
                var isServiceRequest = TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest");
                if (formIdentifier.EndsWith(requestSuffix, StringComparison.Ordinal) && isServiceRequest)
                    formIdentifier = formIdentifier[..^requestSuffix.Length] + "Form";

                add((t, m) => GenerateForm(t, formScriptAttr, formIdentifier, m), formIdentifier);

                EnqueueTypeMembers(type);

                if (isServiceRequest)
                    add(GenerateBasicType);

                return;
            }

            var columnsScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute");
            if (columnsScriptAttr != null)
            {
                add((t, m) => GenerateColumns(t, columnsScriptAttr, m));
                EnqueueTypeMembers(type);
                return;
            }

            if (TypingsUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null ||
                TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute") != null)
            {
                add(GeneratePermissionKeys);
                return;
            }
            
            add(GenerateBasicType);
        }
    }
}