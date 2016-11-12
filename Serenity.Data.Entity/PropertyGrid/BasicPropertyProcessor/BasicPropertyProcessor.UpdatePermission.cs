using Serenity.ComponentModel;
using Serenity.Data;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetUpdatePermission(IPropertySource source, PropertyItem item)
        {
            if (source.Property != null)
            {
                var attr = source.Property.GetAttribute<UpdatePermissionAttribute>(false);
                if (attr != null)
                {
                    if (attr.Permission != "*")
                        item.UpdatePermission = attr.Permission ?? "?";

                    return;
                }
            }

            if (!ReferenceEquals(null, source.BasedOnField))
            {
                if (source.BasedOnField.UpdatePermission != null &&
                    source.BasedOnField.UpdatePermission != "*")
                    item.UpdatePermission = source.BasedOnField.UpdatePermission;
            }
        }
    }
}