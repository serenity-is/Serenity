namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetRequired(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<RequiredAttribute>();
        if (attr != null)
        {
            if (attr.IsRequired)
                item.Required = true;
        }
        else if (source.BasedOnField is not null &&
            (source.BasedOnField.Flags & FieldFlags.NotNull) == FieldFlags.NotNull)
        {
            item.Required = true;
        }
    }
}