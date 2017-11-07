using Serenity.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private FormCssClassAttribute formCssClassPrior;

        private void SetFormCssClass(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<FormCssClassAttribute>() ?? formCssClassPrior;

            if (attr != null)
            {
                item.FormCssClass = attr.Value;
                formCssClassPrior = attr.UntilNext ? attr : null;
            }
        }
    }
}