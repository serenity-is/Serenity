namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        static readonly HashSet<string> lookupEditorBaseOptions;
        static readonly HashSet<string> serviceLookupEditorBaseOptions;

        static ClientTypesGenerator()
        {
            lookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            lookupEditorBaseOptions.AddRange(typeof(LookupEditorBaseAttribute)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => x.Name));

            serviceLookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            serviceLookupEditorBaseOptions.AddRange(typeof(ServiceLookupEditorAttribute)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => x.Name));
        }

        protected override void GenerateAll()
        {
            var generatedTypes = new HashSet<string>();

            foreach (var tsType in tsTypes)
            {
                if (generatedTypes.Contains(tsType.Key))
                    continue;

                if (tsType.Value.IsDeclaration == true)
                    continue;

                GenerateType(tsType.Value);
            }
        }

        private static string GetNamespace(string ns)
        {
            if (ns == "Serenity")
                return "Serenity.ComponentModel";

            return ns;
        }

        private void GenerateType(ExternalType type)
        {
            bool isEditorType = IsEditorType(type);
            bool isFormatterType = IsFormatterType(type);

            if (!isEditorType && !isFormatterType)
                return;

            AppendUsings(new string[] {
                "Serenity",
                "Serenity.ComponentModel",
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.ComponentModel"
            });

            sb.AppendLine();

            var ns = GetNamespace(type.Namespace);
            string name = type.Name + "Attribute";

            if (!string.IsNullOrEmpty(ns))
            {
                cw.Indented("namespace ");
                sb.AppendLine(ns);

                cw.InBrace(delegate
                {
                    if (isEditorType)
                        GenerateEditor(type, name);
                    else if (isFormatterType)
                        GenerateFormatter(type, name);
                });
            }
            else
            {
                if (isEditorType)
                    GenerateEditor(type, name);
                else if (isFormatterType)
                    GenerateFormatter(type, name);
            }

            AddFile(RemoveRootNamespace(ns, name) + ".cs");
        }
    }
}
