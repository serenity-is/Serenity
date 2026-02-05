namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetReadPermission(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<ReadPermissionAttribute>(false);
            if (attr != null)
            {
                if (attr.Permission != SpecialPermissionKeys.Public)
                    item.ReadPermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.ReadPermission != null &&
                source.BasedOnField.ReadPermission != SpecialPermissionKeys.Public)
                item.ReadPermission = source.BasedOnField.ReadPermission;
        }
    }
}