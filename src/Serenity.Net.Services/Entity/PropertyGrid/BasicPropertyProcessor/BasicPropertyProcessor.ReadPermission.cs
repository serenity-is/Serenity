namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetReadPermission(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<ReadPermissionAttribute>(false);
            if (attr != null)
            {
                if (attr.Permission != "*")
                    item.ReadPermission = attr.Permission ?? "?";

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if (source.BasedOnField.ReadPermission != null &&
                source.BasedOnField.ReadPermission != "*")
                item.ReadPermission = source.BasedOnField.ReadPermission;
        }
    }
}