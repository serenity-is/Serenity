namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetCategory(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<CategoryAttribute>();
        if (attr != null)
        {
            if (attr is IHasTextKey hasTextKey && !string.IsNullOrEmpty(hasTextKey.TextKey))
            {
                item.Category = hasTextKey.TextKey;
                return;
            }

            item.Category = GetLocalizableTextValue<CategoryAttribute>(source, attr.Category,
                () => "Categories." + attr.Category, inferTextKey: attr is not IHasTextKey);
        }
        else if (source.GetAttribute<TabAttribute>() is null && Items?.Count > 0)
        {
            item.Category = Items[^1].Category;
        }
        else
            item.Category = null;
    }
}