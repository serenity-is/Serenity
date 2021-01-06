using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetCategory(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<CategoryAttribute>();
            if (attr != null)
                item.Category = attr.Category;
            else if (Items != null && Items.Count > 0)
                item.Category = Items[^1].Category;
        }
    }
}