namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetRecordViews(IPropertySource source, PropertyItem item)
    {
        foreach (var attr in source.GetAttributes<RecordViewTypeAttribute>())
        {
            string? placement = null;
            if (attr.Placement != RecordViewPlacement.Trigger)
            {
                placement = attr.Placement.ToString();
                placement = placement[..1].ToLowerInvariant() + placement[1..];
            }

            // the attributes of the property come before the ones of the field of its row, so one of a columns or
            // form class replaces the same view of the row
            if (item.RecordViews?.Any(x => x.Type == attr.ViewType && x.Placement == placement) == true)
                continue;

            var info = new RecordViewInfo
            {
                Type = attr.ViewType,
                Placement = placement
            };

            attr.SetParams(info.Params);

            // the id of the record that the view shows, e.g. CustomerID for CustomerCompanyName, by the join of the
            // property, as for an edit link
            var idField = attr.IdField ?? AutoDetermineIdField(source.BasedOnField);
            if (idField != null)
                info.Params["idField"] = idField;

            (item.RecordViews ??= []).Add(info);
        }
    }
}
