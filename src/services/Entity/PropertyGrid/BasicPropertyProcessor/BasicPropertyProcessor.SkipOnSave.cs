namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetSkipOnSave(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<SkipOnSaveAttribute>();
        if (attr != null)
            item.SkipOnSave = true;
    }
}