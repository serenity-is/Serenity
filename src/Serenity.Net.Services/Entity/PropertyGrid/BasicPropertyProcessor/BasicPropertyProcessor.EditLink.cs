namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetEditLink(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<EditLinkAttribute>();
        if (attr == null)
            return;

        if (attr.Value)
            item.EditLink = true;

        if (attr.ItemType != null)
            item.EditLinkItemType = attr.ItemType;

        if (attr.IdField != null)
            item.EditLinkIdField = attr.IdField;

        if (attr.CssClass != null)
            item.EditLinkCssClass = attr.CssClass;

        if (item.EditLinkItemType != null &&
            item.EditLinkIdField == null)
        {
            item.EditLinkIdField = AutoDetermineIdField(source.BasedOnField);
        }
    }

    private static string AutoDetermineIdField(Field basedOnField)
    {
        if (basedOnField is null)
            return null;

        Field idField;

        if (basedOnField.Join == null && (basedOnField.ReferencedAliases == null || basedOnField.ReferencedAliases.Count != 1))
            return null;

        if (basedOnField.Join == null)
        {
            idField = basedOnField.Fields.FirstOrDefault(x => x.ForeignJoinAlias != null &&
                (x.TextualField == basedOnField.PropertyName ||
                 x.TextualField == basedOnField.Name));
        }
        else
        {
            var joinName = basedOnField.Join != null ? basedOnField.Join.Name : basedOnField.ReferencedAliases.Single();
            idField = basedOnField.Fields.FirstOrDefault(x => x.ForeignJoinAlias != null &&
                x.ForeignJoinAlias.Name == joinName);
        }

        return idField is null ? null : (idField.PropertyName ?? idField.Name);
    }
}