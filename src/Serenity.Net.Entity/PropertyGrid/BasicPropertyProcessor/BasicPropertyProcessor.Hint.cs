namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetHint(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<HintAttribute>();
        if (attr != null)
            item.Hint = attr.Hint;
    }
}