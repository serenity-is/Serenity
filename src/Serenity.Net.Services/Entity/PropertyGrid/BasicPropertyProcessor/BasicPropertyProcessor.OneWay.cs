namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetOneWay(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<OneWayAttribute>();
        if (attr != null)
            item.OneWay = true;
    }
}