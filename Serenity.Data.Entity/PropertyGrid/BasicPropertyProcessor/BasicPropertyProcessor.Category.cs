using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetCategory(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<CategoryAttribute>();
            if (attr != null) { 
                item.Category = attr.Category;
                item.Collapsible = attr.Collapsible;
                item.Expanded = attr.Expanded;
            }
            else if (Items != null && Items.Count > 0)
                item.Category = Items[Items.Count - 1].Category;
        }
    }
}