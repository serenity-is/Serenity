namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetShowSelection(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<ShowSelectionAttribute>();
        if (attr != null && !attr.Value)
            item.ShowSelection = false;
    }
}