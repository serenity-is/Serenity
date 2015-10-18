using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetSortOrder(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<SortOrderAttribute>();
            if (attr != null && attr.SortOrder != 0)
                item.SortOrder = attr.SortOrder;
        }
    }
}