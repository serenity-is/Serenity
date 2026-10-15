namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetHint(IPropertySource source, PropertyItem item)
    {
        HintAttribute? attr = source.GetAttribute<HintAttribute>();
        if (attr != null)
        {
            if (attr is IHasTextKey hasTextKey && !string.IsNullOrEmpty(hasTextKey.TextKey))
            {
                item.Hint = hasTextKey.TextKey;
                return;
            }

            item.Hint = GetLocalizableTextValue<HintAttribute>(source, attr.Hint, 
                () => source.Name + "_Hint", inferTextKey: attr is not IHasTextKey);
        }
    }
}