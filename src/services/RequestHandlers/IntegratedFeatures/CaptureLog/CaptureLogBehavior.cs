namespace Serenity.Services;

/// <summary>
/// Capture log behavior
/// </summary>
public class CaptureLogBehavior : BaseSaveDeleteBehaviorAsync, ISaveBehaviorSync, IDeleteBehaviorSync,
    IUndeleteBehaviorAsync, IUndeleteBehaviorSync, IImplicitBehavior
{
    private CaptureLogAttribute captureLogAttr;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (row is not IIdRow)
            return false;

        captureLogAttr = row.GetType().GetCustomAttribute<CaptureLogAttribute>();
        return captureLogAttr != null;
    }

    /// <inheritdoc/>
    public virtual void OnAudit(IDeleteRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        var newRow = GetDeletedLogRow(handler.Row);
        Log(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier());
    }

    /// <inheritdoc/>
    public override Task OnAuditAsync(IDeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (handler.Row == null)
            return Task.CompletedTask;

        var newRow = GetDeletedLogRow(handler.Row);
        return LogAsync(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier(), cancellationToken);
    }

    /// <inheritdoc/>
    public virtual void OnAudit(IUndeleteRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        var newRow = GetUndeletedLogRow(handler.Row);
        if (newRow == null)
            return;

        Log(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier());
    }

    /// <inheritdoc/>
    public virtual Task OnAuditAsync(IUndeleteRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (handler.Row == null)
            return Task.CompletedTask;

        var newRow = GetUndeletedLogRow(handler.Row);
        if (newRow == null)
            return Task.CompletedTask;

        return LogAsync(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier(), cancellationToken);
    }

    private static IRow GetDeletedLogRow(IRow row)
    {
        // if row is not actually deleted, but set to deleted by a flag, log it as if it is an update operation
        if (row is IIsActiveDeletedRow isActiveDeletedRow)
        {
            var newRow = row.Clone();
            ((IIsActiveDeletedRow)newRow).IsActiveField[newRow] = -1;
            return newRow;
        }
        else if (row is IIsDeletedRow isDeletedRow)
        {
            var newRow = row.Clone();
            ((IIsDeletedRow)newRow).IsDeletedField[newRow] = true;
            return newRow;
        }

        return null;
    }

    private static IRow GetUndeletedLogRow(IRow row)
    {
        var newRow = row.Clone();

        // log it as if it is an update operation
        if (row is IIsActiveDeletedRow isActiveDeletedRow)
        {
            ((IIsActiveDeletedRow)newRow).IsActiveField[newRow] = 1;
        }
        else if (row is IIsDeletedRow isDeletedRow)
        {
            ((IIsDeletedRow)newRow).IsDeletedField[newRow] = true;
        }
        else if (row is IDeleteLogRow deleteLogRow)
        {
            ((IDeleteLogRow)newRow).DeleteUserIdField.AsObject(newRow, null);
            ((IDeleteLogRow)newRow).DeleteDateField.AsObject(newRow, null);
        }
        else
            return null;

        return newRow;
    }

    /// <inheritdoc/>
    public virtual void OnAudit(ISaveRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        if (handler.IsCreate)
        {
            Log(handler.UnitOfWork, null, handler.Row, handler.Context.User?.GetIdentifier());

            return;
        }

        if (HasAnyChanged(handler))
            Log(handler.UnitOfWork, handler.Old, handler.Row, handler.Context.User?.GetIdentifier());
    }

    /// <inheritdoc/>
    public override Task OnAuditAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (handler.Row == null)
            return Task.CompletedTask;

        if (handler.IsCreate)
            return LogAsync(handler.UnitOfWork, null, handler.Row, handler.Context.User?.GetIdentifier(), cancellationToken);

        if (HasAnyChanged(handler))
            return LogAsync(handler.UnitOfWork, handler.Old, handler.Row, handler.Context.User?.GetIdentifier(), cancellationToken);

        return Task.CompletedTask;
    }

    private static bool HasAnyChanged(ISaveRequestHandler handler)
    {
        foreach (var field in handler.Row.GetTableFields())
        {
            if (handler.Row is IInsertDateRow insertDateRow && ReferenceEquals(insertDateRow.InsertDateField, field))
                continue;

            if (handler.Row is IInsertUserIdRow insertUserIdRow && ReferenceEquals(insertUserIdRow.InsertUserIdField, field))
                continue;

            if (handler.Row is IUpdateDateRow updateDateRow && ReferenceEquals(updateDateRow.UpdateDateField, field))
                continue;

            if (handler.Row is IUpdateUserIdRow updateUserIdRow && ReferenceEquals(updateUserIdRow.UpdateUserIdField, field))
                continue;

            if (field.IndexCompare(handler.Old, handler.Row) != 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Logs a capture log operation
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="old">Old entity</param>
    /// <param name="row">New entity</param>
    /// <param name="userId">User ID</param>
    /// <exception cref="ArgumentNullException"><paramref name="old"/> and <paramref name="row"/> are both <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Capture log row type does not implement ICaptureLogRow interface</exception>
    public void Log(IUnitOfWork uow, IRow old, IRow row, object userId)
    {
        var context = PrepareLog(uow, old, row, userId);

        if (BuildCloseActiveUpdate(context).Execute(uow.Connection, ExpectedRows.Ignore) > 1)
            throw new InvalidOperationException($"Capture log has more than one active instance " +
                $"for ID {context.MappedIdField.AsObject(context.LogRow)}?!");

        uow.Connection.Insert(context.LogRow);

        if (context.OperationType == CaptureOperationType.Before)
        {
            var updateLogRow = BuildBeforeInsertRow(context);
            uow.Connection.Insert(updateLogRow);
        }
    }

    /// <summary>
    /// Asynchronously logs a capture log operation
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="old">Old entity</param>
    /// <param name="row">New entity</param>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="old"/> and <paramref name="row"/> are both <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Capture log row type does not implement ICaptureLogRow interface</exception>
    public async Task LogAsync(IUnitOfWork uow, IRow old, IRow row, object userId,
        CancellationToken cancellationToken = default)
    {
        var context = PrepareLog(uow, old, row, userId);

        if (await BuildCloseActiveUpdate(context).ExecuteAsync(uow.Connection, ExpectedRows.Ignore,
                cancellationToken: cancellationToken).ConfigureAwait(false) > 1)
            throw new InvalidOperationException($"Capture log has more than one active instance " +
                $"for ID {context.MappedIdField.AsObject(context.LogRow)}?!");

        await uow.Connection.InsertAsync(context.LogRow, cancellationToken).ConfigureAwait(false);

        if (context.OperationType == CaptureOperationType.Before)
        {
            var updateLogRow = BuildBeforeInsertRow(context);
            await uow.Connection.InsertAsync(updateLogRow, cancellationToken).ConfigureAwait(false);
        }
    }

    private static SqlUpdate BuildCloseActiveUpdate(LogContext context)
    {
        return new SqlUpdate(context.LogRow.Table)
            .Set(context.LogRow.ValidUntilField, context.Now)
            .WhereEqual(context.MappedIdField, context.MappedIdField.AsSqlValue(context.LogRow))
            .WhereEqual(context.LogRow.ValidUntilField, CaptureLogConsts.UntilMax);
    }

    private static ICaptureLogRow BuildBeforeInsertRow(LogContext context)
    {
        var updateLogRow = (ICaptureLogRow)context.LogRow.CreateNew();
        updateLogRow.TrackAssignments = true;
        updateLogRow.ChangingUserIdField.AsInvariant(updateLogRow, context.UserId);
        updateLogRow.OperationTypeField[updateLogRow] = CaptureOperationType.Update;
        updateLogRow.ValidFromField[updateLogRow] = context.Now;
        updateLogRow.ValidUntilField[updateLogRow] = CaptureLogConsts.UntilMax;
        context.CopyCapturedFields(context.Row, updateLogRow);
        return updateLogRow;
    }

    private LogContext PrepareLog(IUnitOfWork uow, IRow old, IRow row, object userId)
    {
        if (old == null && row == null)
            throw new ArgumentNullException("old");

        var now = DateTime.Now;
        var rowInstance = row ?? old;
        var rowType = rowInstance.GetType();
        var logRow = (Activator.CreateInstance(captureLogAttr.LogRow) as ICaptureLogRow) ??
            throw new InvalidOperationException($"Capture log table {captureLogAttr.LogRow.FullName} " +
                $"for {rowType.FullName} doesn't implement ICaptureLogRow interface!");

        var rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(rowInstance.EnumerateTableFields(), x => x.Name);
        var logFieldPrefixLength = PrefixHelper.DeterminePrefixLength(logRow.EnumerateTableFields(), x => x.Name);
        var mappedIdFieldName = captureLogAttr.MappedIdField ?? rowInstance.IdField.Name;
        var mappedIdField = logRow.FindField(mappedIdFieldName) ?? throw new InvalidOperationException($"Can't locate capture log table " +
                $"mapped ID field for {logRow.Table}!");
        logRow.TrackAssignments = true;
        logRow.ChangingUserIdField.AsInvariant(logRow, userId);

        var operationType = old == null ? CaptureOperationType.Insert :
            (row == null ? CaptureOperationType.Delete : CaptureOperationType.Before);

        logRow.OperationTypeField[logRow] = operationType;
        logRow.ValidFromField[logRow] = now;

        IEnumerable<Tuple<Field, Field>> enumerateCapturedFields()
        {
            foreach (var logField in logRow.Fields)
            {
                if (!logField.IsTableField())
                    continue;

                if (ReferenceEquals(logRow.ChangingUserIdField, logField) ||
                    ReferenceEquals(logRow.ValidFromField, logField) ||
                    ReferenceEquals(logRow.ValidUntilField, logField) ||
                    ReferenceEquals(logRow.OperationTypeField, logField) ||
                    ReferenceEquals(logRow.IdField, logField))
                    continue;

                if (ReferenceEquals(logField, mappedIdField))
                    yield return new Tuple<Field, Field>(logField, rowInstance.IdField);
                else
                {
                    var name = logField.Name[logFieldPrefixLength..];
                    name = rowInstance.IdField.Name[..rowFieldPrefixLength] + name;
                    var match = rowInstance.FindField(name) ?? throw new InvalidOperationException($"Can't find match in the row for log table field {name}!");
                    yield return new Tuple<Field, Field>(logField, match);
                }
            }
        }

        void copyCapturedFields(IRow source, IRow target)
        {
            foreach (var tuple in enumerateCapturedFields())
            {
                var value = tuple.Item2.AsObject(source);
                tuple.Item1.AsObject(target, value);
            }
        }

        if (operationType == CaptureOperationType.Insert)
        {
            logRow.ValidUntilField[logRow] = CaptureLogConsts.UntilMax;
            copyCapturedFields(row, logRow);
        }
        else
        {
            logRow.ValidUntilField[logRow] = now;
            copyCapturedFields(old, logRow);
        }

        return new LogContext(logRow, row, operationType, now, userId, copyCapturedFields, mappedIdField);
    }

    private sealed class LogContext(ICaptureLogRow logRow, IRow row, CaptureOperationType operationType,
        DateTime now, object userId, Action<IRow, IRow> copyCapturedFields, Field mappedIdField)
    {
        public ICaptureLogRow LogRow { get; } = logRow;
        public IRow Row { get; } = row;
        public CaptureOperationType OperationType { get; } = operationType;
        public DateTime Now { get; } = now;
        public object UserId { get; } = userId;
        public Action<IRow, IRow> CopyCapturedFields { get; } = copyCapturedFields;
        public Field MappedIdField { get; } = mappedIdField;
    }
}
