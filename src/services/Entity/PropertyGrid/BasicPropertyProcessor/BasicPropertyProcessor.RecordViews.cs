namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetRecordViews(IPropertySource source, PropertyItem item)
    {
        foreach (var attr in source.GetAttributes<RecordViewTypeAttribute>())
        {
            // the id of the record that the view shows, e.g. CustomerID for CustomerCompanyName, by the join of the
            // property, as for an edit link
            var info = RecordViewInfo.FromAttribute(attr, AutoDetermineIdField(source.BasedOnField));

            // the attributes of the property come before the ones of the field of its row, so one of a columns or
            // form class replaces the same view of the row
            if (item.RecordViews?.Any(x => x.Type == info.Type && x.Placement == info.Placement) == true)
                continue;

            (item.RecordViews ??= []).Add(info);
        }
    }
}
