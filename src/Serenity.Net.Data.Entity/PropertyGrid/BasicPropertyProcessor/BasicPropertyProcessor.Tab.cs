using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetTab(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<TabAttribute>();
            if (attr != null)
                item.Tab = attr.Value;
            else if (Items != null && Items.Count > 0)
                item.Tab = Items[^1].Tab;
        }
    }
}