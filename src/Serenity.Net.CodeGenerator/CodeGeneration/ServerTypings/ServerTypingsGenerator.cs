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
            void addNamespaceTyping(Action<TypeDefinition> action, string fileIdentifier = null)
            {
                cw.Indented("namespace ");
                var codeNamespace = GetNamespace(type);
                sb.Append(codeNamespace);
                cw.InBrace(delegate
                {
                    action(type);
                });
                AddFile(RemoveRootNamespace(codeNamespace, (fileIdentifier ?? type.Name) + ".ts"));
            }

            if (type.IsEnum())
            {
                addNamespaceTyping(GenerateEnum);
                return;
            }

            if (TypingsUtils.IsSubclassOf(type, "Microsoft.AspNetCore.Mvc", "Controller") ||
                TypingsUtils.IsSubclassOf(type, "System.Web.Mvc", "Controller"))
            {
                var controllerIdentifier = GetControllerIdentifier(type);
                addNamespaceTyping(t => GenerateService(t, controllerIdentifier, false), controllerIdentifier);
                return;
            }

            var formScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "FormScriptAttribute");
            if (formScriptAttr != null)
            {
                var formIdentifier = type.Name;
                var isServiceRequest = TypingsUtils.IsSubclassOf(type, "Serenity.Services", "ServiceRequest");
                if (formIdentifier.EndsWith(requestSuffix, StringComparison.Ordinal) && isServiceRequest)
                    formIdentifier = formIdentifier[..^requestSuffix.Length] + "Form";

                addNamespaceTyping(t => GenerateForm(t, formScriptAttr, formIdentifier), formIdentifier);

                EnqueueTypeMembers(type);

                if (isServiceRequest)
                    addNamespaceTyping(type => GenerateBasicType(type, false));

                return;
            }

            var columnsScriptAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "ColumnsScriptAttribute");
            if (columnsScriptAttr != null)
            {
                addNamespaceTyping(t => GenerateColumns(t, columnsScriptAttr));
                EnqueueTypeMembers(type);
                return;
            }

            if (TypingsUtils.GetAttr(type, "Serenity.Extensibility", "NestedPermissionKeysAttribute") != null ||
                TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "NestedPermissionKeysAttribute") != null)
            {
                addNamespaceTyping(GeneratePermissionKeys);
                return;
            }
            
            addNamespaceTyping(t => GenerateBasicType(t, false));
        }
    }
}