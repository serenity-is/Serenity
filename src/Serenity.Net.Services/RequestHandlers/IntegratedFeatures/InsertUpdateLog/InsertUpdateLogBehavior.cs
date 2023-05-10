namespace Serenity.Services;

/// <summary>
/// Behavior class that handles assignment of relevant fields 
/// for rows with <see cref="IInsertLogRow"/> 
/// and <see cref="IUpdateLogRow"/> interfaces
/// </summary>
public class UpdateInsertLogBehavior : BaseSaveBehavior, IImplicitBehavior
{
    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        return row is IUpdateLogRow || row is IInsertLogRow;
    }

    /// <inheritdoc/>
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