namespace Serenity.Services
{
    public class DisplayOrderFilterHelper
    {
        public static BaseCriteria GetDisplayOrderFilterFor(IRow row)
        {
            var flt = Criteria.Empty;
            if (row as IParentIdRow != null)
                flt &= new Criteria((row as IParentIdRow).ParentIdField) == Convert.ToInt64((row as IParentIdRow).ParentIdField.AsObject(row));

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
}