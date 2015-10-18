using Serenity.ComponentModel;
using Serenity.Data;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetRequired(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<RequiredAttribute>();
            if (attr != null)
            {
                if (attr.IsRequired)
                    item.Required = true;
            }

            if (!ReferenceEquals(null, source.BasedOnField) &&
                (source.BasedOnField.Flags & FieldFlags.NotNull) == FieldFlags.NotNull)
            {
                item.Required = true;
            }
        }
    }
}