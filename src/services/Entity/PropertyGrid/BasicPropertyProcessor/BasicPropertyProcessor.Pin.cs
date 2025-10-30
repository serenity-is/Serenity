namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetPin(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<PinColumnAttribute>();
        if (attr != null)
            item.Pin = attr.Value;
    }
}