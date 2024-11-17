namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetDefaultValue(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<DefaultValueAttribute>(false);
            if (attr != null)
            {
                item.DefaultValue = attr.Value;
                return;
            }
        }

        if (source.BasedOnField is not null && source.BasedOnField.DefaultValue != null)
            item.DefaultValue = source.BasedOnField.DefaultValue;
    }
}