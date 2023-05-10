namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator : ImportGeneratorBase
{
    static readonly HashSet<string> lookupEditorBaseOptions;
    static readonly HashSet<string> serviceLookupEditorBaseOptions;

    static ClientTypesGenerator()
    {
        lookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Async",
            "AutoComplete",
            "CascadeField",
            "CascadeValue",
            "Delimited",
            "DialogType",
            "FilterField",
            "FilterValue",
            "InplaceAdd",
            "InplaceAddPermission",
            "LookupKey",
            "MinimumResultsForSearch",
            "Multiple",
            "OpenDialogAsPanel"
        };

        var lookupEditorBaseAttr = Type.GetType("Serenity.ComponentModel.LookupEditorBaseAttribute, Serenity.Net.Core");
        if (lookupEditorBaseAttr != null)
        {
            foreach (var p in lookupEditorBaseAttr
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                lookupEditorBaseOptions.Add(p.Name);
        }

        serviceLookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AutoComplete",
            "CascadeField",
            "CascadeValue",
            "ColumnSelection",
            "Delimited",
            "DialogType",
            "ExcludeColumns",
            "FilterField",
            "FilterValue",
            "IdField",
            "TextField",
            "IncludeColumns",
            "IncludeDeleted",
            "InplaceAdd",
            "InplaceAddPermission",
            "ItemType",
            "MinimumResultsForSearch",
            "Multiple",
            "OpenDialogAsPanel",
            "PageSize",
            "Service",
            "Sort"
        };

        var serviceLookupEditorBaseAttr = Type.GetType("Serenity.ComponentModel.ServiceLookupEditorBaseAttribute, Serenity.Net.Core");
        if (serviceLookupEditorBaseAttr != null)
        {
            foreach (var p in serviceLookupEditorBaseAttr
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                serviceLookupEditorBaseOptions.Add(p.Name);
        }
    }

    protected override void GenerateAll()
    {
        cw.IsCSharp = true;
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

        foreach (var defaultUsing in new string[] {
            "Serenity",
            "Serenity.ComponentModel",
            "System",
            "System.Collections",
            "System.Collections.Generic",
            "System.ComponentModel"
        })
        {
            cw.Using(defaultUsing);
        }

        var ns = GetNamespace(type.Namespace);

        var key = type.Attributes?.FirstOrDefault(x =>
            x.Arguments?.Count > 0 &&
            !string.IsNullOrEmpty(x.Arguments[0]?.Value as string) &&
            (
                string.Equals(x.Type, "registerClass", StringComparison.Ordinal) ||
                string.Equals(x.Type, "registerEditor", StringComparison.Ordinal) ||
                string.Equals(x.Type, "registerFormatter", StringComparison.Ordinal) ||
                x.Type?.EndsWith(".registerClass", StringComparison.Ordinal) == true ||
                x.Type?.EndsWith(".registerEditor", StringComparison.Ordinal) == true ||
                x.Type?.EndsWith(".registerFormatter", StringComparison.Ordinal) == true)
            )?.Arguments[0].Value as string ??
            type.Fields?.FirstOrDefault(x =>
                x.IsStatic == true &&
                x.Name == "__typeName" &&
                x.Value is string)?.Value as string;

        if (string.IsNullOrEmpty(ns))
        {
            if (key != null)
            {
                var idx = key.LastIndexOf('.');
                if (idx > 0)
                    ns = key[..idx];
            }

            if (string.IsNullOrEmpty(ns))
                ns = RootNamespaces.FirstOrDefault(x => x != "Serenity") ?? "App";
        }
        else
            key = null;

        string name = type.Name + "Attribute";

        cw.InNamespace(ns, () =>
        {
            if (isEditorType)
                GenerateEditor(type, name, key);
            else if (isFormatterType)
                GenerateFormatter(type, name, key);
        });

        AddFile(RemoveRootNamespace(ns, name) + ".cs");
    }
}
