namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetCategory(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<CategoryAttribute>();
        if (attr != null)
        {
            item.Category = GetLocalizableTextValue<CategoryAttribute>(source, attr.Category,
                () => "Categories." + attr.Category);
        }
        else if (Items != null && Items.Count > 0)
            item.Category = Items[^1].Category;
    }
}