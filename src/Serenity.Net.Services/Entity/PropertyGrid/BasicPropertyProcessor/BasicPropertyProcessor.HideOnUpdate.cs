namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetHideOnUpdate(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<HideOnUpdateAttribute>();
        if (attr != null && attr.Value)
            item.HideOnUpdate = true;
    }
}