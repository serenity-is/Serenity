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
            flt &= new Criteria(parentRow.ParentIdField) == Convert.ToInt64(parentRow.ParentIdField.AsObject(row));

        if (row is IIsActiveRow activeRow)
            flt &= new Criteria(activeRow.IsActiveField) >= 0;
        else
        {
            if (row is IIsDeletedRow deletedRow)
                flt &= deletedRow.IsDeletedField == 0;
        }

        return flt;
    }
}