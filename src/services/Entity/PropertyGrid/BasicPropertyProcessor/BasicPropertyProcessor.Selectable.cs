namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetSelectable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<SelectableAttribute>();
        if (attr != null)
        {
            item.Selectable = false;
        }
    }
}