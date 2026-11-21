namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Imports the client side classes of the record views of a property, i.e. the attributes deriving from
    /// RecordViewTypeAttribute, on the columns class, on the field of the row, or on an annotation of it, so the grids
    /// that have them load them with themselves. The type key of a view class is the Key constant of its attribute, else
    /// the one it gives the constructor of its base, else the one given to RecordViewTypeAttribute itself.
    /// </summary>
    private void ReferenceRecordViewTypes(TypeDefinition type, PropertyDefinition item, PropertyDefinition? basedOnField,
        TypeDefinition? basedOnRow, IEnumerable<AnnotationTypeInfo>? rowAnnotations, HashSet<(string group, string key)> referencedTypeKeys,
        List<(string group, string alias)> referencedTypeAliases)
    {
        var attributes = item.GetAttributesWithInterfaces(type).AsEnumerable();
        if (rowAnnotations != null)
            foreach (var annotationType in rowAnnotations)
                if (annotationType.PropertyByName.TryGetValue(item.Name, out PropertyDefinition? annotation))
                    attributes = attributes.Concat(annotation!.GetAttributesWithIntrinsic());
        if (basedOnField != null)
            attributes = attributes.Concat(basedOnField.GetAttributesWithInterfaces(basedOnRow!));

        ReferenceRecordViewTypes(attributes, referencedTypeKeys, referencedTypeAliases);
    }

    /// <summary>
    /// Imports the client side classes of the record views of a columns class as a whole, e.g. a card view that is an
    /// alternate view of its rows, so the grids that have them load them with themselves.
    /// </summary>
    private void ReferenceRecordViewTypes(TypeDefinition type, HashSet<(string group, string key)> referencedTypeKeys,
        List<(string group, string alias)> referencedTypeAliases)
    {
        ReferenceRecordViewTypes(type.GetAttributes(), referencedTypeKeys, referencedTypeAliases);
    }

    private void ReferenceRecordViewTypes(IEnumerable<CustomAttribute> attributes,
        HashSet<(string group, string key)> referencedTypeKeys, List<(string group, string alias)> referencedTypeAliases)
    {
        foreach (var attr in attributes)
        {
            if (attr.AttributeType() is not { } attributeType)
                continue;

            string? viewKey;
            if (attributeType.FullNameOf() == "Serenity.ComponentModel.RecordViewTypeAttribute")
                viewKey = attr.ConstructorArguments()?.Count == 1 ? attr.ConstructorArguments()[0].Value as string : null;
            else if (TypingsUtils.IsSubclassOf(attributeType, "Serenity.ComponentModel", "RecordViewTypeAttribute"))
                viewKey = TypingsUtils.GetAttributeKeyViaKeyConstant(attributeType) ??
                    GetAttributeKeyViaBaseCtorCall(attributeType);
            else
                continue;

            if (string.IsNullOrEmpty(viewKey) || !referencedTypeKeys.Add(("RecordView", viewKey!)))
                continue;

            var viewType = FindTypeInLookup(modularRecordViewTypeByKey, viewKey!, "RecordView", containingAssembly: null);
            if (viewType != null)
                referencedTypeAliases.Add(("RecordView", ReferenceScriptType(viewType)));
        }
    }
}
