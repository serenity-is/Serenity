namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetFocusable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<FocusableAttribute>();
        if (attr != null)
        {
            item.Focusable = false;
        }
    }
}