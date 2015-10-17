using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetPlaceholder(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<PlaceholderAttribute>();
            if (attr != null)
                item.Placeholder = attr.Value;
        }
    }
}