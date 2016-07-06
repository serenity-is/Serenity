using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetSorting(IPropertySource source, PropertyItem item)
        {
            var sortOrderAttr = source.GetAttribute<SortOrderAttribute>();
            if (sortOrderAttr != null && sortOrderAttr.SortOrder != 0)
                item.SortOrder = sortOrderAttr.SortOrder;

            var sortableAttr = source.GetAttribute<SortableAttribute>();
            if (sortableAttr != null && !sortableAttr.Value)
                item.Sortable = false;
        }
    }
}