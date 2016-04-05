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

        static ClientTypesGenerator()
        {
            lookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            lookupEditorBaseOptions.AddRange(typeof(LookupEditorBaseAttribute)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => x.Name));
        }

        protected override void GenerateAll()
        {
            foreach (var type in ssByScriptName.Values)
                GenerateType(type);

            foreach (var p in tsTypes)
                if (!ssByScriptName.ContainsKey(p.Key))
                    GenerateType(p.Value);
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
