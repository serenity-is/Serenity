namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetAllowHide(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<AllowHideAttribute>();
        if (attr != null && attr.Value == false)
            item.AllowHide = false;
    }
}