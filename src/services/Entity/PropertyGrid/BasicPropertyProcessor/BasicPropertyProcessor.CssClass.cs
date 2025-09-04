namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetCssClass(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<CssClassAttribute>();
        if (attr != null)
            item.CssClass = attr.CssClass;

        var hattr = source.GetAttribute<HeaderCssClassAttribute>();
        if (hattr != null)
            item.HeaderCssClass = hattr.Value;
    }
}