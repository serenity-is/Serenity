namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetGrouping(IPropertySource source, PropertyItem item)
    {
        var groupOrderAttr = source.GetAttribute<GroupOrderAttribute>();
        if (groupOrderAttr != null && groupOrderAttr.GroupOrder != 0)
            item.GroupOrder = groupOrderAttr.GroupOrder;
    }
}