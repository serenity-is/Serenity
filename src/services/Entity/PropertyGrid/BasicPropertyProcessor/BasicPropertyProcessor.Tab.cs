namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetTab(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<TabAttribute>();
        if (attr != null)
        {
            if (attr is IHasTextKey hasTextKey && !string.IsNullOrEmpty(hasTextKey.TextKey))
            {
                item.Tab = hasTextKey.TextKey;
                return;
            }

            item.Tab = GetLocalizableTextValue<TabAttribute>(source, attr.Value,
                () => "Tabs." + attr.Value, inferTextKey: attr is not IHasTextKey);
        }
        else if (Items != null && Items.Count > 0)
            item.Tab = Items[^1].Tab;
    }
}