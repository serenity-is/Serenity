using Serenity.ComponentModel;
using Serenity.Data;

namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        public override void Process(IPropertySource source, PropertyItem item)
        {
            SetAlignment(source, item);
            SetCategory(source, item);
            SetCssClass(source, item);
            SetDefaultValue(source, item);
            SetEditLink(source, item);
            SetFiltering(source, item);
            SetFormatting(source, item);
            SetHint(source, item);
            SetInsertable(source, item);
            SetOneWay(source, item);
            SetPlaceholder(source, item);
            SetReadOnly(source, item);
            SetRequired(source, item);
            SetResizable(source, item);
            SetSortOrder(source, item);
            SetTitle(source, item);
            SetUpdatable(source, item);
            SetVisible(source, item);
            SetWidth(source, item);
        }

        public override int Priority
        {
            get { return 10; }
        }
    }
}