namespace Serenity.Services;

/// <summary>
/// Display order related helper methods
/// </summary>
public class DisplayOrderFilterHelper
{
    /// <summary>
    /// Gets display order criteria for a row instance
    /// </summary>
    /// <param name="row">Row class</param>
    public static BaseCriteria GetDisplayOrderFilterFor(IRow row)
    {
        BaseCriteria flt = Criteria.Empty;
        if (row is IParentIdRow parentRow)
        {
            // Use the parent id's own field type instead of forcing it to long, so Guid/string
            // keys work, and map a null parent id to IS NULL instead of coercing it to 0.
            var parentId = parentRow.ParentIdField.AsObject(row);
            flt &= parentId is null
                ? parentRow.ParentIdField.IsNull()
                : parentRow.ParentIdField == new ValueCriteria(parentId);
        }

        if (row is IIsActiveRow activeRow)
            flt &= activeRow.IsActiveField >= 0;
        else
        {
            if (row is IIsDeletedRow deletedRow)
                flt &= deletedRow.IsDeletedField == 0;
        }

        return flt;
    }
}