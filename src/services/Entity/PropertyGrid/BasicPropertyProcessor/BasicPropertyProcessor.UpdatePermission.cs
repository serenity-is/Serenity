namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetUpdatePermission(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<UpdatePermissionAttribute>(false);
            if (attr != null)
            {
                if (attr.Permission != SpecialPermissionKeys.Public)
                    item.UpdatePermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.UpdatePermission != null &&
                source.BasedOnField.UpdatePermission != SpecialPermissionKeys.LoggedIn)
                item.UpdatePermission = source.BasedOnField.UpdatePermission;
        }
    }
}