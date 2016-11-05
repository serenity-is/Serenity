using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetCollapsible(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<CollapsibleAttribute>();
            if (attr != null)
            {
                item.Collapsible = true;
                item.Collapsed = attr.Collapsed;
            }
        }
    }
}