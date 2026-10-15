namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetPlaceholder(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<PlaceholderAttribute>();
        if (attr != null)
        {
            if (attr is IHasTextKey hasTextKey && !string.IsNullOrEmpty(hasTextKey.TextKey))
            {
                item.Placeholder = hasTextKey.TextKey;
                return;
            }

            item.Placeholder = GetLocalizableTextValue<PlaceholderAttribute>(source, attr.Value,
                () => source.Name + "_Placeholder", inferTextKey: attr is not IHasTextKey);
        }
    }
}