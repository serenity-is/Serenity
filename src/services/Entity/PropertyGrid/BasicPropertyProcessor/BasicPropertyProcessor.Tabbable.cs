namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetTabbable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<TabbableAttribute>();
        if (attr != null && !attr.Value)
            item.Tabbable = false;
    }
}