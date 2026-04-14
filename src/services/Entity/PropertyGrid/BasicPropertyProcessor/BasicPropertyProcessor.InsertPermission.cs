namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetInsertPermission(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<InsertPermissionAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (attr.Permission != SpecialPermissionKeys.Public)
                item.InsertPermission = attr.Permission ?? SpecialPermissionKeys.LoggedIn;

            return;
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.InsertPermission != null &&
                source.BasedOnField.InsertPermission != SpecialPermissionKeys.Public)
                item.InsertPermission = source.BasedOnField.InsertPermission;
        }
    }
}