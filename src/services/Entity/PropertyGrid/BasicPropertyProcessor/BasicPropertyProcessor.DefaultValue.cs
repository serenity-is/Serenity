namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetDefaultValue(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<DefaultValueAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            item.DefaultValue = attr.Value;
            return;
        }

        if (source.BasedOnField is not null && source.BasedOnField.DefaultValue != null)
            item.DefaultValue = source.BasedOnField.DefaultValue;
    }
}