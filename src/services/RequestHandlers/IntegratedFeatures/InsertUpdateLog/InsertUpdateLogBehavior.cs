namespace Serenity.Services;

/// <summary>
/// Behavior class that handles assignment of relevant fields 
/// for rows with <see cref="IInsertLogRow"/> 
/// and <see cref="IUpdateLogRow"/> interfaces
/// </summary>
public class UpdateInsertLogBehavior : BaseSaveBehaviorAsync, ISaveBehaviorSync, IImplicitBehavior
{
    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        return row is IInsertDateRow || row is IInsertUserIdRow || 
            row is IUpdateDateRow || row is IUpdateUserIdRow;
    }

    /// <inheritdoc/>
    public virtual void OnSetInternalFields(ISaveRequestHandler handler)
    {
        var row = handler.Row;
        var insertDateRow = row as IInsertDateRow;
        var insertUserIdRow = row as IInsertUserIdRow;

        if (row is IUpdateDateRow updateDateRow && (handler.IsUpdate || insertDateRow == null))
        {
            updateDateRow.UpdateDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                updateDateRow.UpdateDateField.DateTimeKind);
        }
        else if (insertDateRow != null && handler.IsCreate)
        {
            insertDateRow.InsertDateField[row] = DateTimeField.ToDateTimeKind(DateTime.Now,
                insertDateRow.InsertDateField.DateTimeKind);
        }

        if (row is IUpdateUserIdRow updateLogRow && (handler.IsUpdate || insertUserIdRow == null))
        {
            updateLogRow.UpdateUserIdField.AsInvariant(row, handler.Context.User?.GetIdentifier());
        }
        else if (insertUserIdRow != null && handler.IsCreate)
        {
            insertUserIdRow.InsertUserIdField.AsInvariant(row, handler.Context.User?.GetIdentifier());
        }
    }

    /// <inheritdoc/>
    public override Task OnSetInternalFieldsAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        OnSetInternalFields(handler);
        return Task.CompletedTask;
    }
}
