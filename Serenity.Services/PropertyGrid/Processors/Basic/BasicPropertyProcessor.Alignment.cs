using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetCssClass(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<CssClassAttribute>();
            if (attr != null)
                item.CssClass = attr.CssClass;
        }
    }
}