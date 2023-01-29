namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetCollapsible(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<CollapsibleAttribute>();
        if (attr != null && attr.Value)
        {
            item.Collapsible = true;
            if (attr.Collapsed)
                item.Collapsed = true;
        }
    }
}