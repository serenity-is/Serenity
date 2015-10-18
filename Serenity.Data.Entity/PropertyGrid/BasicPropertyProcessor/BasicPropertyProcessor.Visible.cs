using Serenity.ComponentModel;
using System.ComponentModel;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetVisible(IPropertySource source, PropertyItem item)
        {
            var attr = source.GetAttribute<VisibleAttribute>();
            if (attr != null && attr.Value == false)
                item.Visible = false;
        }
    }
}