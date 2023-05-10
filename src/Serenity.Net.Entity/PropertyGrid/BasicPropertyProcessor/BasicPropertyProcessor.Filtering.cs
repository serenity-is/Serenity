namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetFiltering(IPropertySource source, PropertyItem item)
    {
        var filterOnlyAttr = source.GetAttribute<FilterOnlyAttribute>();
        var notFilterableAttr = source.GetAttribute<NotFilterableAttribute>();

        if (filterOnlyAttr != null && filterOnlyAttr.Value)
            item.FilterOnly = true;

        if (notFilterableAttr != null && notFilterableAttr.Value)
            item.NotFilterable = true;

        if (item.NotFilterable == true)
            return;

        var quickFilterAttr = source.GetAttribute<QuickFilterAttribute>();
        if (quickFilterAttr != null)
        {
            item.QuickFilter = true;
            if (quickFilterAttr.Separator)
                item.QuickFilterSeparator = true;

            if (!string.IsNullOrEmpty(quickFilterAttr.CssClass))
                item.QuickFilterCssClass = quickFilterAttr.CssClass;
        }

        var basedOnField = source.BasedOnField;
        if (basedOnField is not null &&
            notFilterableAttr == null)
        {
            if (basedOnField.Flags.HasFlag(FieldFlags.DenyFiltering) ||
                basedOnField.Flags.HasFlag(FieldFlags.NotMapped))
            {
                item.NotFilterable = true;
            }
        }

        Field idField;
        string idFieldName;
        var filteringIdField = source.GetAttribute<FilteringIdFieldAttribute>();
        if (filteringIdField != null)
        {
            idFieldName = filteringIdField.Value;
            idField = basedOnField.Fields.FindFieldByPropertyName(idFieldName) ?? basedOnField.Fields.FindField(idFieldName);
        }
        else
        {
            idFieldName = AutoDetermineIdField(basedOnField);

            idField = null;
            if (idFieldName != null)
            {
                idField = basedOnField.Fields.FindFieldByPropertyName(idFieldName) ?? basedOnField.Fields.FindField(idFieldName);
                if (idField is null ||
                    (idField.TextualField != basedOnField.PropertyName &&
                     idField.TextualField != basedOnField.Name))
                {
                    idField = null;
                    idFieldName = null;
                }
            }
        }

        var valueType = source.ValueType;

        var filteringTypeAttr = source.GetAttribute<FilteringTypeAttribute>() ??
            idField.GetAttribute<FilteringTypeAttribute>();


        if (filteringTypeAttr == null)
        {
            var editorAttr = source.GetAttribute<EditorTypeAttribute>() ??
                idField.GetAttribute<EditorTypeAttribute>();

            void copyParamsFromEditor(string[] keys)
            {
                var prm = new Dictionary<string, object>();
                editorAttr.SetParams(prm);
                SetServiceLookupParams(editorAttr, prm);
                foreach (var key in keys)
                {
                    if (prm.TryGetValue(key, out object o))
                        item.FilteringParams[key] = o;
                }
            }

            if (idFieldName != null)
            {
                item.FilteringParams["idField"] = idFieldName;
                item.FilteringIdField = idFieldName;
            }

            if (editorAttr != null && !standardFilteringEditors.Contains(editorAttr.EditorType))
            {
                if (editorAttr is LookupEditorAttribute)
                {
                    item.FilteringType = "Lookup";
                    copyParamsFromEditor(lookupCopyToFilterParams);
                }
                else if (editorAttr is ServiceLookupEditorAttribute)
                {
                    item.FilteringType = "ServiceLookup";
                    copyParamsFromEditor(serviceLookupCopyToFilterParams);
                }
                else
                {
                    item.FilteringType = "Editor";
                    item.FilteringParams["editorType"] = editorAttr.EditorType;
                    item.FilteringParams["useLike"] = source.ValueType == typeof(string);
                    if (editorAttr is LookupEditorBaseAttribute leba &&
                        leba.Async == true)
                        item.FilteringParams["async"] = true;
                }
            }
            else if (source.EnumType != null)
            {
                item.FilteringType = "Enum";
                item.FilteringParams["enumKey"] = EnumMapper.GetEnumTypeKey(source.EnumType);
            }
            else if (valueType == typeof(DateTime))
            {
                if (basedOnField is not null &&
                    basedOnField is DateTimeField dtf &&
                    !dtf.DateOnly)
                {
                    item.FilteringType = "DateTime";
                }
                else
                    item.FilteringType = "Date";
            }
            else if (valueType == typeof(bool))
                item.FilteringType = "Boolean";
            else if (valueType == typeof(decimal) ||
                valueType == typeof(double) ||
                valueType == typeof(float))
            {
                item.FilteringType = "Decimal";
            }
            else if (valueType == typeof(int) ||
                valueType == typeof(short) ||
                valueType == typeof(long))
            {
                item.FilteringType = "Integer";
            }
            else
                item.FilteringType = "String";
        }
        else
        {
            item.FilteringType = filteringTypeAttr.FilteringType;
            filteringTypeAttr.SetParams(item.FilteringParams);

            if (item.FilteringType == "Editor")
            {
                if (!item.FilteringParams.ContainsKey("editorType"))
                {
                    var editorAttr = source.GetAttribute<EditorTypeAttribute>() ??
                        idField.GetAttribute<EditorTypeAttribute>();

                    if (editorAttr != null)
                        item.FilteringParams["editorType"] = editorAttr.EditorType;
                }

                if (!item.FilteringParams.ContainsKey("useLike"))
                {
                    if (valueType == typeof(string))
                        item.FilteringParams["useLike"] = true;
                }
            }

            if (item.FilteringParams.TryGetValue("idField", out object idFieldObj) && idFieldObj is string)
                item.FilteringIdField = (idFieldObj as string).TrimToNull();
            else
                item.FilteringIdField = idFieldName;
        }

        var displayFormatAttr = source.GetAttribute<DisplayFormatAttribute>();
        if (displayFormatAttr != null)
            item.FilteringParams["displayFormat"] = displayFormatAttr.Value;

        foreach (FilteringOptionAttribute param in
            idField.GetAttributes<FilteringOptionAttribute>().Concat(
            source.GetAttributes<FilteringOptionAttribute>()))
        {
            var key = param.Key;
            if (key != null &&
                key.Length >= 1)
                key = key[..1].ToLowerInvariant() + key[1..];

            if (key == "idField")
                item.FilteringIdField = (param.Value as string) ?? item.FilteringIdField;

            item.FilteringParams[key] = param.Value;
        }

        foreach (QuickFilterOptionAttribute param in
            idField.GetAttributes<QuickFilterOptionAttribute>().Concat(
            source.GetAttributes<QuickFilterOptionAttribute>()))
        {
            var key = param.Key;
            if (key != null &&
                key.Length >= 1)
                key = key[..1].ToLowerInvariant() + key[1..];

            item.QuickFilterParams[key] = param.Value;
        }
    }

    private static readonly string[] lookupCopyToFilterParams = new string[]
    {
        "lookupKey",
        "async"
    };

    private static readonly string[] serviceLookupCopyToFilterParams = new string[]
    {
        "service",
        "idField",
        "textField",
        "pageSize",
        "sort",
        "columnSelection",
        "includeColumns",
        "excludeColumns",
        "includeDeleted"
    };

    private static readonly HashSet<string> standardFilteringEditors = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Date",
        "Serenity.Date",
        "DateTime",
        "Serenity.DateTime",
        "Boolean",
        "Serenity.Boolean",
        "Decimal",
        "Integer",
        "Serenity.Integer"
    };

}