namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetResizable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<ResizableAttribute>();
        if (attr != null && attr.Value == false)
            item.Resizable = false;
    }
}