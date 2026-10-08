namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetResizable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<ResizableAttribute>();
        if (attr != null && attr.Value == false)
            item.Resizable = false;
        else
        {
            var fixedWidth = source.GetAttribute<FixedWidthAttribute>();
            if (fixedWidth != null &&
                fixedWidth.Value == fixedWidth.Min &&
                fixedWidth.Min == fixedWidth.Max)
                item.Resizable = false;
        }
    }
}