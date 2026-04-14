namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetUpdatePermission(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<UpdatePermissionAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (attr.Permission != SpecialPermissionKeys.Public)
                item.UpdatePermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

            return;
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.UpdatePermission != null &&
                source.BasedOnField.UpdatePermission != SpecialPermissionKeys.LoggedIn)
                item.UpdatePermission = source.BasedOnField.UpdatePermission;
        }
    }
}