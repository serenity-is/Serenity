using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetFormCssClass(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<FormCssClassAttribute>();
            if (attr != null)
                item.FormCssClass = attr.Value;
        }
    }
}