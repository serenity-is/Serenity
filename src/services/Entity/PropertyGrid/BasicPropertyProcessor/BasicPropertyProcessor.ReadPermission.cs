namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetReadPermission(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<ReadPermissionAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (attr.Permission != SpecialPermissionKeys.Public)
                item.ReadPermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

            return;
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.ReadPermission != null &&
                source.BasedOnField.ReadPermission != SpecialPermissionKeys.Public)
                item.ReadPermission = source.BasedOnField.ReadPermission;
        }
    }
}