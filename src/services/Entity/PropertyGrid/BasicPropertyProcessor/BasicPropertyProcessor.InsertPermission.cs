namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetInsertPermission(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<InsertPermissionAttribute>(false);
            if (attr != null)
            {
                if (attr.Permission != SpecialPermissionKeys.Public)
                    item.InsertPermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.InsertPermission != null &&
                source.BasedOnField.InsertPermission != SpecialPermissionKeys.Public)
                item.InsertPermission = source.BasedOnField.InsertPermission;
        }
    }
}