namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetAlignment(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<AlignmentAttribute>();
        if (attr != null)
            item.Alignment = attr.Value;
    }
}