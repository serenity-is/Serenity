namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetReadOnly(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<ReadOnlyAttribute>();
        if (attr != null && attr.IsReadOnly)
            item.ReadOnly = true;
    }
}