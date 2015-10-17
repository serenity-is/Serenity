using Serenity.ComponentModel;
using Serenity.Data;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetUpdatable(IPropertySource source, PropertyItem item)
        {
            if (source.Member != null)
            {
                var attr = source.Member.GetAttribute<UpdatableAttribute>(false);
                if (attr != null)
                {
                    if (!attr.Value)
                        item.Updatable = false;

                    return;
                }
            }

            if (!ReferenceEquals(null, source.BasedOnField))
            {
                if ((source.BasedOnField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                    item.Updatable = false;
            }
        }
    }
}