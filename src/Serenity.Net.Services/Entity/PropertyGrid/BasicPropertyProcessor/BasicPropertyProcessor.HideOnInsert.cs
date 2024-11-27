namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetHideOnInsert(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<HideOnInsertAttribute>();
        if (attr != null && attr.Value)
            item.HideOnInsert = true;
    }
}