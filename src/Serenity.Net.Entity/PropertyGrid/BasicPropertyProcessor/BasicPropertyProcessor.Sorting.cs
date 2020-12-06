using Serenity.ComponentModel;
using Serenity.Data;

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
            if (sortableAttr != null)
            {
                if (!sortableAttr.Value)
                    item.Sortable = false;

                return;
            }

            if (source.BasedOnField is object &&
                source.BasedOnField.Flags.HasFlag(FieldFlags.NotMapped))
                item.Sortable = false;
        }
    }
}