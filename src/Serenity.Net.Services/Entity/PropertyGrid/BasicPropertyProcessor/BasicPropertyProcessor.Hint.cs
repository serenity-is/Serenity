namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetHint(IPropertySource source, PropertyItem item)
    {
        HintAttribute attr = source.GetAttribute<HintAttribute>();
        if (attr != null)
        {
            item.Hint = GetLocalizableTextValue<HintAttribute>(source, attr.Hint, 
                () => source.Property?.Name + "_Hint");
        }
    }
}