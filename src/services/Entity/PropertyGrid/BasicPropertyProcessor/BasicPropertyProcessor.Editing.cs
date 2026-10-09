namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetEditing(IPropertySource source, PropertyItem item)
    {
        var editorTypeAttr = source.GetAttribute<EditorTypeAttribute>();

        if (editorTypeAttr == null)
        {
            item.EditorType = AutoDetermineEditorType(source.ValueType, source.EnumType, item.EditorParams);
        }
        else
        {
            item.EditorType = editorTypeAttr.EditorType;
            editorTypeAttr.SetParams(item.EditorParams);
            if (item.EditorType == "Lookup" &&
                !item.EditorParams.ContainsKey("lookupKey"))
            {
                var distinct = source.GetAttribute<DistinctValuesEditorAttribute>();
                if (distinct != null)
                {
                    string? prefix = null;
                    if (distinct.RowType != null)
                    {
                        if (!distinct.RowType.IsInterface &&
                            !distinct.RowType.IsAbstract &&
                            typeof(IRow).IsAssignableFrom(distinct.RowType))
                        {
                            prefix = ((IRow)Activator.CreateInstance(distinct.RowType)!)
                                .Fields.LocalTextPrefix;
                        }
                    }
                    else
                    {
                        var isRow = source.ReflectedType != null &&
                            !source.ReflectedType.IsAbstract &&
                            !source.ReflectedType.IsInterface &&
                            typeof(IRow).IsAssignableFrom(source.ReflectedType);

                        if (!isRow)
                        {
                            if (source.BasedOnField is not null)
                                prefix = source.BasedOnField.Fields.LocalTextPrefix;
                        }
                        else
                            prefix = ((IRow)Activator.CreateInstance(source.ReflectedType!)!)
                                .Fields.LocalTextPrefix;
                    }

                    if (prefix != null)
                    {
                        var propertyName = string.IsNullOrEmpty(distinct.PropertyName) ?
                                (source.BasedOnField is not null ?
                                    (source.BasedOnField.PropertyName ?? source.BasedOnField.Name) :
                                item.Name) : distinct.PropertyName;

                        if (!string.IsNullOrEmpty(propertyName))
                            item.EditorParams["lookupKey"] = "Distinct." + prefix + "." + propertyName;
                    }
                }
            }

        }

        if (source.EnumType != null)
            item.EditorParams["enumKey"] = EnumMapper.GetEnumTypeKey(source.EnumType);

        var dtka = source.GetAttribute<DateTimeKindAttribute>();
        if (dtka != null && dtka.Value != DateTimeKind.Unspecified)
            item.EditorParams["useUtc"] = true;

        if (source.BasedOnField is not null)
        {
            if (dtka == null &&
                source.BasedOnField is DateTimeField dtf &&
                dtf.DateTimeKind != DateTimeKind.Unspecified)
                item.EditorParams["useUtc"] = true;

            if (item.EditorType == "Decimal" &&
                (source.BasedOnField is DoubleField ||
                 source.BasedOnField is SingleField ||
                 source.BasedOnField is DecimalField) &&
                source.BasedOnField.Size > 0 &&
                source.BasedOnField.Scale < source.BasedOnField.Size &&
                !item.EditorParams.ContainsKey("minValue") &&
                !item.EditorParams.ContainsKey("maxValue"))
            {
                string minVal = new('0', source.BasedOnField.Size - source.BasedOnField.Scale);
                if (source.BasedOnField.Scale > 0)
                    minVal += "." + new string('0', source.BasedOnField.Scale);
                string maxVal = minVal.Replace('0', '9');

                if ((item.EditorParams.ContainsKey("allowNegatives") &&
                     Convert.ToBoolean(item.EditorParams["allowNegatives"] ?? false) == true) ||
                    (!item.EditorParams.ContainsKey("allowNegatives") &&
                    DecimalEditorAttribute.AllowNegativesByDefault))
                    minVal = "-" + maxVal;

                item.EditorParams["minValue"] = minVal;
                item.EditorParams["maxValue"] = maxVal;
            }
            else if (item.EditorType == "Integer" &&
                (source.BasedOnField is Int32Field ||
                 source.BasedOnField is Int16Field ||
                 source.BasedOnField is Int64Field) &&
                !item.EditorParams.ContainsKey("minValue") &&
                !item.EditorParams.ContainsKey("maxValue"))
            {
                item.EditorParams["maxValue"] = source.BasedOnField is Int16Field ? short.MaxValue
                    : source.BasedOnField is Int32Field ? int.MaxValue :
                    source.BasedOnField is Int64Field ? long.MaxValue : null;

                if ((item.EditorParams.ContainsKey("allowNegatives") &&
                     Convert.ToBoolean(item.EditorParams["allowNegatives"] ?? false) == true) ||
                    !item.EditorParams.ContainsKey("allowNegatives") &&
                    DecimalEditorAttribute.AllowNegativesByDefault &&
                    item.EditorParams["maxValue"] != null)
                    item.EditorParams["minValue"] = -Convert.ToInt64(item.EditorParams["maxValue"]) - 1;
            }
            else if (source.BasedOnField.Size > 0)
            {
                item.EditorParams["maxLength"] = source.BasedOnField.Size;
                item.MaxLength = source.BasedOnField.Size;
            }
        }

        var maxLengthAttr = source.GetAttribute<MaxLengthAttribute>();
        if (maxLengthAttr != null)
        {
            item.MaxLength = maxLengthAttr.MaxLength;
            item.EditorParams["maxLength"] = maxLengthAttr.MaxLength;
        }

        foreach (EditorOptionAttribute param in source.GetAttributes<EditorOptionAttribute>())
        {
            var key = param.Key;
            if (key != null &&
                key.Length >= 1)
                key = key[..1].ToLowerInvariant() + key[1..];

            item.EditorParams[key!] = param.Value;
        }

        SetServiceLookupParams(editorTypeAttr, item.EditorParams);
    }

    private static void SetServiceLookupParams(EditorTypeAttribute? editorTypeAttr, Dictionary<string, object?> editorParams)
    {
        if (editorTypeAttr is not ServiceLookupEditorBaseAttribute sle)
            return;

        bool validateInferredFields = editorTypeAttr.GetType() == typeof(ServiceLookupEditorAttribute);
        var itemType = sle.ItemType ??
            sle.EndpointType?.GetCustomAttribute<ConnectionKeyAttribute>()?.SourceType;
        var serviceType = itemType ?? sle.EndpointType;

        if (!editorParams.ContainsKey("service"))
        {
            if (sle.EndpointType != null &&
                ServiceLookupEditorAttribute.TryGetServiceFromEndpoint(sle.EndpointType, sle.ActionName) is string fromEndpoint)
            {
                editorParams["service"] = fromEndpoint;
            }
            else if (serviceType != null)
            {
                editorParams["service"] = ServiceLookupEditorAttribute.AutoServiceFor(serviceType, sle.ActionName);
            }
        }

        ListRequestCapabilities? effectiveCapabilities = null;
        if (editorParams.TryGetValue("capabilities", out var configuredCapabilities) &&
            configuredCapabilities is ListRequestCapabilities configured)
        {
            effectiveCapabilities = configured;
            editorParams["capabilities"] = configured;
        }

        if (effectiveCapabilities is null)
        {
            var endpointType = sle.EndpointType;
            if (endpointType is null &&
                itemType is not null &&
                typeof(IRow).IsAssignableFrom(itemType))
            {
                endpointType = ServiceLookupEditorAttribute.TryGetEndpointForRow(
                    itemType, sle.ActionName, out _);
            }

            if (endpointType is not null &&
                ServiceLookupEditorAttribute.TryGetEndpointActionMethod(endpointType, sle.ActionName) is MethodInfo actionMethod)
            {
                var actionCapabilities = actionMethod.GetCustomAttribute<ListRequestCapabilitiesAttribute>(inherit: true);
                var parameters = actionMethod.GetParameters();
                var requestParameter = parameters.FirstOrDefault(parameter =>
                    typeof(ServiceRequest).IsAssignableFrom(parameter.ParameterType)) ??
                    parameters.FirstOrDefault(parameter =>
                        parameter.ParameterType != typeof(CancellationToken));

                if (actionCapabilities is not null)
                {
                    effectiveCapabilities = actionCapabilities.Capabilities &
                        ~actionCapabilities.Exclude;
                    editorParams["capabilities"] = effectiveCapabilities.Value;
                }
                else
                {
                    if (requestParameter is null)
                    {
                        effectiveCapabilities = ListRequestCapabilities.None;
                        editorParams["capabilities"] = ListRequestCapabilities.None;
                    }
                    else if (requestParameter.ParameterType != typeof(ListRequest))
                    {
                        effectiveCapabilities = InferListRequestCapabilities(requestParameter.ParameterType);
                        editorParams["capabilities"] = effectiveCapabilities.Value;
                    }
                }
            }
        }

        bool idFieldMissing = !editorParams.ContainsKey("idField");
        bool textFieldMissing = !editorParams.ContainsKey("textField");

        var resultEndpointType = sle.EndpointType;
        if (resultEndpointType is null &&
            itemType is not null &&
            typeof(IRow).IsAssignableFrom(itemType))
        {
            resultEndpointType = ServiceLookupEditorAttribute.TryGetEndpointForRow(
                itemType, sle.ActionName, out _);
        }

        var resultAction = resultEndpointType is null ? null :
            ServiceLookupEditorAttribute.TryGetEndpointActionMethod(resultEndpointType, sle.ActionName);
        if (resultAction is not null &&
            TryGetPrimitiveListItemType(resultAction.ReturnType) is not null)
        {
            if (idFieldMissing)
                editorParams["idField"] = "Value";
            if (textFieldMissing)
                editorParams["textField"] = "Value";
        }

        if (itemType == null)
            return;

        bool isRowType = typeof(IRow).IsAssignableFrom(itemType);
        bool containsTextUnsupported = effectiveCapabilities is { } capabilities &&
            (capabilities & ListRequestCapabilities.ContainsText) == 0;

        if (containsTextUnsupported &&
            !isRowType &&
            !editorParams.ContainsKey("quickSearchFields"))
        {
            var quickSearchFields = itemType.GetProperties()
                .Where(property => property.GetMethod is { IsPublic: true, IsStatic: false } &&
                    property.GetIndexParameters().Length == 0)
                .Where(property => property.GetCustomAttribute<Serenity.Data.Mapping.QuickSearchAttribute>() is
                    { IsExplicit: false })
                .Select(property => property.Name)
                .ToArray();

            if (quickSearchFields.Length > 0)
                editorParams["quickSearchFields"] = quickSearchFields;
        }

        // Respect explicit column options; for example, Lookup selection can include lookup columns without an inferred list.
        bool columnsMissing = isRowType &&
            !editorParams.ContainsKey("includeColumns") &&
            !editorParams.ContainsKey("columnSelection");

        if (!idFieldMissing && !textFieldMissing && !columnsMissing)
            return;

        if (!isRowType)
        {
            var properties = itemType.GetProperties()
                .Where(property => property.GetMethod is { IsPublic: true, IsStatic: false } &&
                    property.GetIndexParameters().Length == 0)
                .ToArray();

            if (idFieldMissing)
            {
                var idProperty = FindPropertyByAttributeOrName(properties,
                    property => property.GetCustomAttribute<IdPropertyAttribute>() != null,
                    "Id", "Key", "Code", "Value");
                if (idProperty != null)
                    editorParams["idField"] = idProperty.Name;
            }

            if (textFieldMissing)
            {
                var textProperty = FindPropertyByAttributeOrName(properties,
                    property => property.GetCustomAttribute<NamePropertyAttribute>() != null,
                    "Name", "DisplayName", "Text");
                if (textProperty != null)
                    editorParams["textField"] = textProperty.Name;
            }

            if (properties.Length == 1 &&
                properties[0].PropertyType == typeof(string))
            {
                if (idFieldMissing && !editorParams.ContainsKey("idField"))
                    editorParams["idField"] = properties[0].Name;
                if (textFieldMissing && !editorParams.ContainsKey("textField"))
                    editorParams["textField"] = properties[0].Name;
            }

            if (validateInferredFields)
                ThrowIfLookupFieldsMissing(itemType, editorParams);
            return;
        }

        bool success = false;
        if (!itemType.IsAbstract &&
            !itemType.IsInterface &&
            itemType.GetConstructors().Any(x => x.GetParameters().Length == 0))
        {
            // Intentionally not cached: row metadata/data can vary per call
            // (e.g. multitenancy, per-request fields), so the instance is created on demand.
            try
            {
                var rowInstance = Activator.CreateInstance(itemType) as IRow;
                if (idFieldMissing)
                {
                    var idField = rowInstance?.IdField;
                    var idFieldName = idField?.PropertyName ?? idField?.Name;
                    if (idFieldName != null)
                        editorParams["idField"] = idFieldName;
                }

                if (textFieldMissing)
                {
                    var nameField = rowInstance?.NameField;
                    var textFieldName = nameField?.PropertyName ?? nameField?.Name;
                    if (textFieldName != null)
                        editorParams["textField"] = textFieldName;
                }

                if (columnsMissing)
                {
                    editorParams["includeColumns"] = rowInstance?.Fields
                        .Where(x => x.GetAttribute<LookupIncludeAttribute>() != null)
                        .Select(x => x.PropertyName ?? x.Name)
                        .ToArray() ?? [];
                }

                success = true;
            }
            catch
            {
                // fallback to reflection-based approach if instantiation fails
            }
        }

        if (!success)
        {
            if (idFieldMissing)
            {
                var idFieldName = itemType.GetProperties().FirstOrDefault(x =>
                    x.GetCustomAttribute<IdPropertyAttribute>() != null)?.Name;
                if (idFieldName != null)
                    editorParams["idField"] = idFieldName;
            }
            if (textFieldMissing)
            {
                var textFieldName = itemType.GetProperties().FirstOrDefault(x =>
                    x.GetCustomAttribute<NamePropertyAttribute>() != null)?.Name;
                if (textFieldName != null)
                    editorParams["textField"] = textFieldName;
            }
            if (columnsMissing)
            {
                // play safe as other service types may not support includeColumns property
                editorParams["includeColumns"] = itemType.GetProperties().Where(x => x.GetCustomAttribute<LookupIncludeAttribute>() != null).Select(x => x.Name).ToArray();
            }
        }

        if (validateInferredFields)
            ThrowIfLookupFieldsMissing(itemType, editorParams);
    }

    private static ListRequestCapabilities InferListRequestCapabilities(Type requestType)
    {
        return GetListRequestCapabilities(requestType);
    }

    private static ListRequestCapabilities GetListRequestCapabilities(Type requestType)
    {
        var requestProperties = requestType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.SetMethod is { IsPublic: true } &&
                property.GetIndexParameters().Length == 0)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var capabilities = ListRequestCapabilities.None;
        foreach (var property in typeof(ListRequest).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            if (property.DeclaringType == typeof(ListRequest) &&
                requestProperties.Contains(property.Name) &&
                Enum.TryParse(property.Name, out ListRequestCapabilities capability))
                capabilities |= capability;

        return capabilities;
    }

    private static Type? TryGetPrimitiveListItemType(Type returnType)
    {
        if (returnType.IsGenericType &&
            (returnType.GetGenericTypeDefinition() == typeof(Task<>) ||
             returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
            returnType = returnType.GetGenericArguments()[0];

        if (!returnType.IsGenericType)
            return null;

        var genericType = returnType.GetGenericTypeDefinition();
        if (genericType != typeof(List<>) &&
            genericType != typeof(ListResponse<>))
            return null;

        var itemType = returnType.GetGenericArguments()[0];
        return itemType.IsPrimitive ||
            itemType == typeof(string) ||
            itemType == typeof(decimal) ||
            itemType == typeof(Guid) ||
            itemType.IsEnum ? itemType : null;
    }

    private static void ThrowIfLookupFieldsMissing(Type? itemType,
        Dictionary<string, object?> editorParams)
    {
        var missing = new List<string>();
        if (!editorParams.ContainsKey("idField"))
            missing.Add("IdField");
        if (!editorParams.ContainsKey("textField"))
            missing.Add("TextField");

        if (missing.Count == 0)
            return;

        var typeName = itemType?.FullName ?? "(unknown item type)";
        throw new InvalidOperationException(
            $"Could not determine {string.Join(" and ", missing)} for ServiceLookup items of type '{typeName}'. " +
            "Set the editor fields explicitly or annotate the item properties with IdPropertyAttribute and NamePropertyAttribute.");
    }

    private static PropertyInfo? FindPropertyByAttributeOrName(PropertyInfo[] properties,
        Func<PropertyInfo, bool> predicate, params string[] names)
    {
        var attributed = properties.Where(predicate).Take(2).ToArray();
        return attributed.Length switch
        {
            1 => attributed[0],
            > 1 => null,
            _ => FindPropertyByName(properties, names)
        };
    }

    private static PropertyInfo? FindUniqueProperty(PropertyInfo[] properties,
        Func<PropertyInfo, bool> predicate)
    {
        var matches = properties.Where(predicate).Take(2).ToArray();
        return matches.Length == 1 ? matches[0] : null;
    }

    private static PropertyInfo? FindPropertyByName(PropertyInfo[] properties,
        params string[] names)
    {
        foreach (var name in names)
        {
            var matches = properties.Where(property =>
                string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToArray();

            if (matches.Length == 1)
                return matches[0];

            if (matches.Length > 1)
                return null;
        }

        return null;
    }

    private static string AutoDetermineEditorType(Type valueType, Type? enumType, Dictionary<string, object?> editorParams)
    {
        if (enumType != null)
            return "Enum";
        else if (valueType == typeof(string))
            return "String";
        else if (valueType == typeof(long))
            return "Int64";
        else if (valueType == typeof(int) ||
            valueType == typeof(short))
        {
            if (IntegerEditorAttribute.AllowNegativesByDefault)
                editorParams["allowNegatives"] = true;

            if (valueType == typeof(short))
                editorParams["maxValue"] = short.MaxValue;

            return "Integer";
        }
        else if (valueType == typeof(DateTime) || valueType == typeof(DateOnly))
            return "Date";
        else if (valueType == typeof(TimeSpan))
            return "TimeSpan";
        else if (valueType == typeof(bool))
            return "Boolean";
        else if (valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(float))
        {
            if (DecimalEditorAttribute.AllowNegativesByDefault)
                editorParams["allowNegatives"] = true;
            return "Decimal";
        }
        else
            return "String";
    }
}