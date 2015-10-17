using Serenity.ComponentModel;
using Serenity.Data;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetInsertable(IPropertySource source, PropertyItem item)
        {
            if (source.Member != null)
            {
                var attr = source.Member.GetAttribute<InsertableAttribute>(false);
                if (attr != null)
                {
                    if (!attr.Value)
                        item.Insertable = false;

                    return;
                }
            }

            if (!ReferenceEquals(null, source.BasedOnField))
            {
                if ((source.BasedOnField.Flags & FieldFlags.Insertable) != FieldFlags.Insertable)
                    item.Insertable = false;
            }
        }
    }
}