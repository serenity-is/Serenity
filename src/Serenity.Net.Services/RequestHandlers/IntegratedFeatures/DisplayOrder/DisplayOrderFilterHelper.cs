using Serenity.Data;
using System;

namespace Serenity.Services
{
    public class DisplayOrderFilterHelper
    {
        public static BaseCriteria GetDisplayOrderFilterFor(Row row)
        {
            var flt = Criteria.Empty;
            var parentIdRow = row as IParentIdRow;
            if (parentIdRow != null)
                flt = flt & (new Criteria((Field)parentIdRow.ParentIdField) == Convert.ToInt64(((Field)parentIdRow.ParentIdField).AsObject(row)));

            var activeRow = row as IIsActiveRow;
            if (activeRow != null)
                flt = flt & new Criteria((Field)activeRow.IsActiveField) >= 0;
            else
            {
                var deletedRow = row as IIsDeletedRow;
                if (deletedRow != null)
                    flt = flt & deletedRow.IsDeletedField == 0;
            }

            return flt;
        }
    }
}