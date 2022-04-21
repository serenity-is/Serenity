namespace Serenity.Services
{
    public class UpdateInsertLogBehavior : BaseSaveBehavior, IImplicitBehavior
    {
        public bool ActivateFor(IRow row)
        {
            return row is IUpdateLogRow || row is IInsertLogRow;
        }

        public override void OnSetInternalFields(ISaveRequestHandler handler)
        {
            var row = handler.Row;
            var insertLogRow = row as IInsertLogRow;

            Field field;
            var userId = handler.Context.User?.GetIdentifier();
            if (row is IUpdateLogRow updateLogRow && (handler.IsUpdate || insertLogRow == null))
            {
                updateLogRow.UpdateDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                    updateLogRow.UpdateDateField.DateTimeKind);

                field = updateLogRow.UpdateUserIdField;
                field.AsObject(row, field.ConvertValue(userId, CultureInfo.InvariantCulture));
            }
            else if (insertLogRow != null && handler.IsCreate)
            {
                insertLogRow.InsertDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                    insertLogRow.InsertDateField.DateTimeKind);

                field = insertLogRow.InsertUserIdField;
                field.AsObject(row, field.ConvertValue(userId, CultureInfo.InvariantCulture));
            }
        }
    }
}