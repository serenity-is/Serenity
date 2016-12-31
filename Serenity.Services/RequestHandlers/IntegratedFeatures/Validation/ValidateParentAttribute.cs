using Serenity.Data;

namespace Serenity.Services
{
    public class ValidateParentAttribute : SaveRequestBehaviorAttribute
    {
        public override void OnValidateRequest(ISaveRequestHandler handler)
        {
            base.OnValidateRequest(handler);

            var row = handler.Row;
            var old = handler.Old;
            var isUpdate = old == null;

            var parentIdRow = row as IParentIdRow;
            if (parentIdRow == null)
                return;

            var parentId = parentIdRow.ParentIdField[row];
            if (parentId == null)
                return;

            if (isUpdate && parentId == parentIdRow.ParentIdField[old])
                return;

            var parentIdField = (Field)parentIdRow.ParentIdField;
            if (parentIdField.ForeignTable.IsNullOrEmpty())
                return;

            var foreignRow = RowRegistry.GetConnectionRow(RowRegistry.GetConnectionKey(row), 
                parentIdField.ForeignTable);

            if (foreignRow == null)
                return;

            var idForeign = (IIdRow)foreignRow;
            if (idForeign == null)
                return;

            var isActiveForeign = (IIsActiveRow)foreignRow;
            if (isActiveForeign == null)
                return;

            ServiceHelper.CheckParentNotDeleted(handler.UnitOfWork.Connection, foreignRow.Table, 
                query => query.Where(
                    new Criteria((Field)idForeign.IdField) == parentId.Value &
                    new Criteria(isActiveForeign.IsActiveField) < 0));
        }
    }
}