using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetDefaultValue(IPropertySource source, PropertyItem item)
        {
            if (source.Property != null)
            {
                var attr = source.Property.GetAttribute<DefaultValueAttribute>(false);
                if (attr != null)
                {
                    item.DefaultValue = attr.Value;
                    return;
                }
            }

            if (!ReferenceEquals(null, source.BasedOnField) && source.BasedOnField.DefaultValue != null)
                item.DefaultValue = source.BasedOnField.DefaultValue;
        }
    }
}