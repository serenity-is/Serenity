namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetConditions(IPropertySource source, PropertyItem item)
    {
        static List<PropertyCondition>? conditions<TAttr>(IEnumerable<TAttr> attrs) where TAttr : PropertyConditionAttribute
        {
            var list = attrs.Select(x => x.ToCondition()).ToList();
            return list.Count > 0 ? list : null;
        }

        var visibleWhen = source.GetAttributes<VisibleWhenAttribute>().ToList();
        item.VisibleWhen = conditions(visibleWhen);
        if (visibleWhen.Any(x => x.ClearWhenHidden))
            item.ClearWhenHidden = true;

        item.ReadOnlyWhen = conditions(source.GetAttributes<ReadOnlyWhenAttribute>());
        item.RequiredWhen = conditions(source.GetAttributes<RequiredWhenAttribute>());
    }
}
