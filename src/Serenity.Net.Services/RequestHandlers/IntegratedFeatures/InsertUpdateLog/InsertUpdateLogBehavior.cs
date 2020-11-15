using Serenity.Data;
using System;
using System.Globalization;

namespace Serenity.Services
{
    public class UpdateInsertLogBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        public bool ActivateFor(Row row)
        {
            return row is IUpdateLogRow || row is IInsertLogRow;
        }

        public override void OnSetInternalFields(ISaveRequestHandler handler)
        {
            var row = handler.Row;
            var updateLogRow = row as IUpdateLogRow;
            var insertLogRow = row as IInsertLogRow;

            Field field;

            if (updateLogRow != null && (handler.IsUpdate || insertLogRow == null))
            {
                updateLogRow.UpdateDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                    updateLogRow.UpdateDateField.DateTimeKind);

                if (updateLogRow.UpdateUserIdField.IsIntegerType)
                    updateLogRow.UpdateUserIdField[row] = Authorization.UserId.TryParseID();
                else
                {
                    field = (Field)updateLogRow.UpdateUserIdField;
                    field.AsObject(row, field.ConvertValue(Authorization.UserId, CultureInfo.InvariantCulture));
                }
            }
            else if (insertLogRow != null && handler.IsCreate)
            {
                insertLogRow.InsertDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                    insertLogRow.InsertDateField.DateTimeKind);

                if (insertLogRow.InsertUserIdField.IsIntegerType)
                    insertLogRow.InsertUserIdField[row] = Authorization.UserId.TryParseID();
                else
                {
                    field = (Field)insertLogRow.InsertUserIdField;
                    field.AsObject(row, field.ConvertValue(Authorization.UserId, CultureInfo.InvariantCulture));
                }
            }
        }
    }
}