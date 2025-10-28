namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetSkipOnLoad(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<SkipOnLoadAttribute>();
        if (attr != null)
            item.SkipOnLoad = true;
    }
}