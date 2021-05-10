using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        static HashSet<string> lookupEditorBaseOptions;
        static HashSet<string> serviceLookupEditorBaseOptions;

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

            foreach (var type in ssByScriptName)
            {
                var key = type.Key;
                var ssType = type.Value;
                ExternalType tsType;
                if (ssType.IsDeclaration &&
                    tsTypes.TryGetValue(key, out tsType) &&
                    !tsType.IsDeclaration)
                    continue;

                if (!IsEditorType(ssType) &&
                    !IsFormatterType(ssType))
                    continue;

                generatedTypes.Add(key);

                GenerateType(ssType);
            }

            foreach (var tsType in tsTypes)
            {
                if (generatedTypes.Contains(tsType.Key))
                    continue;

                if (tsType.Value.IsDeclaration)
                    continue;

                GenerateType(tsType.Value);
            }
        }

        private string GetNamespace(string ns)
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

            cw.Indented("namespace ");
            sb.AppendLine(ns);

            cw.InBrace(delegate
            {
                if (isEditorType)
                    GenerateEditor(type, name);
                else if (isFormatterType)
                    GenerateFormatter(type, name);
            });

            AddFile(RemoveRootNamespace(ns, name) + ".cs");
        }
    }
}
