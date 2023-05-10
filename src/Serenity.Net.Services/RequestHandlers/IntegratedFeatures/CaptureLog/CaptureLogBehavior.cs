namespace Serenity.Services;

/// <summary>
/// Capture log behavior
/// </summary>
public class CaptureLogBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IUndeleteBehavior
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
    public override void OnAudit(IDeleteRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        IRow newRow = null;

        // if row is not actually deleted, but set to deleted by a flag, log it as if it is an update operation
        if (handler.Row is IIsActiveDeletedRow)
        {
            newRow = handler.Row.Clone();
            ((IIsActiveDeletedRow)newRow).IsActiveField[newRow] = -1;
        }
        else if (handler.Row is IIsDeletedRow)
        {
            newRow = handler.Row.Clone();
            ((IIsDeletedRow)newRow).IsDeletedField[newRow] = true;
        }

        Log(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier());
    }

    /// <inheritdoc/>
    public void OnAudit(IUndeleteRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        IRow newRow = handler.Row.Clone();

        // log it as if it is an update operation
        if (handler.Row is IIsActiveDeletedRow)
        {
            ((IIsActiveDeletedRow)newRow).IsActiveField[newRow] = 1;
        }
        else if (handler.Row is IIsDeletedRow)
        {
            ((IIsDeletedRow)newRow).IsDeletedField[newRow] = true;
        }
        else if (handler.Row is IDeleteLogRow)
        {
            ((IDeleteLogRow)newRow).DeleteUserIdField.AsObject(newRow, null);
            ((IDeleteLogRow)newRow).DeleteDateField.AsObject(newRow, null);
        }
        else
            return;

        Log(handler.UnitOfWork, handler.Row, newRow, handler.Context.User?.GetIdentifier());
    }

    /// <inheritdoc/>
    public override void OnAudit(ISaveRequestHandler handler)
    {
        if (handler.Row == null)
            return;

        if (handler.IsCreate)
        {
            Log(handler.UnitOfWork, null, handler.Row, handler.Context.User?.GetIdentifier());

            return;
        }

        bool anyChanged = false;
        foreach (var field in handler.Row.GetTableFields())
        {
            if (handler.Row is IInsertLogRow insertLogRow &&
                (ReferenceEquals(insertLogRow.InsertDateField, field) ||
                 ReferenceEquals(insertLogRow.InsertUserIdField, field)))
                continue;

            if (handler.Row is IUpdateLogRow updateLogRow &&
                (ReferenceEquals(updateLogRow.UpdateDateField, field) ||
                 ReferenceEquals(updateLogRow.UpdateUserIdField, field)))
            {
                continue;
            }

            if (field.IndexCompare(handler.Old, handler.Row) != 0)
            {
                anyChanged = true;
                break;
            }
        }

        if (anyChanged)
            Log(handler.UnitOfWork, handler.Old, handler.Row, handler.Context.User?.GetIdentifier());
    }

    /// <summary>
    /// Logs a capture log operation
    /// </summary>
    /// <param name="uow">Unit of work</param>
    /// <param name="old">Old entity</param>
    /// <param name="row">New entity</param>
    /// <param name="userId">User ID</param>
    /// <exception cref="ArgumentNullException">old and row is null</exception>
    /// <exception cref="InvalidOperationException">Capture log row type does not implement ICaptureLogRow interface</exception>
    public void Log(IUnitOfWork uow, IRow old, IRow row, object userId)
    {
        if (old == null && row == null)
            throw new ArgumentNullException("old");

        var now = DateTime.Now;
        var rowInstance = row ?? old;
        var rowType = rowInstance.GetType();
        var logRow = (Activator.CreateInstance(captureLogAttr.LogRow) as ICaptureLogRow) ??
            throw new InvalidOperationException($"Capture log table {captureLogAttr.LogRow.FullName} " +
                $"for {rowType.FullName} doesn't implement ICaptureLogRow interface!");

        var logConnectionKey = logRow.Fields.ConnectionKey;

        var rowFieldPrefixLength = PrefixHelper.DeterminePrefixLength(rowInstance.EnumerateTableFields(), x => x.Name);
        var logFieldPrefixLength = PrefixHelper.DeterminePrefixLength(logRow.EnumerateTableFields(), x => x.Name);
        var mappedIdFieldName = captureLogAttr.MappedIdField ?? rowInstance.IdField.Name;
        var mappedIdField = logRow.FindField(mappedIdFieldName);
        if (mappedIdField is null)
            throw new InvalidOperationException($"Can't locate capture log table " +
                $"mapped ID field for {logRow.Table}!");

        logRow.TrackAssignments = true;
        logRow.ChangingUserIdField.AsObject(logRow, userId == null ? null :
        logRow.ChangingUserIdField.ConvertValue(userId, CultureInfo.InvariantCulture));

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
                    name = rowInstance.IdField.Name.Substring(0, rowFieldPrefixLength) + name;
                    var match = rowInstance.FindField(name);
                    if (match is null)
                        throw new InvalidOperationException($"Can't find match in the row for log table field {name}!");

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

        if (new SqlUpdate(logRow.Table)
                .Set(logRow.ValidUntilField, now)
                .WhereEqual(mappedIdField, mappedIdField.AsSqlValue(logRow))
                .WhereEqual(logRow.ValidUntilField, CaptureLogConsts.UntilMax)
                .Execute(uow.Connection, ExpectedRows.Ignore) > 1)
            throw new InvalidOperationException($"Capture log has more than one active instance " +
                $"for ID {mappedIdField.AsObject(logRow)}?!");

        uow.Connection.Insert(logRow);

        if (operationType == CaptureOperationType.Before)
        {
            logRow = (ICaptureLogRow)logRow.CreateNew();
            logRow.TrackAssignments = true;
            logRow.ChangingUserIdField.AsObject(logRow, userId == null ? null :
                logRow.ChangingUserIdField.ConvertValue(userId, Invariants.NumberFormat));
            logRow.OperationTypeField[logRow] = CaptureOperationType.Update;
            logRow.ValidFromField[logRow] = now;
            logRow.ValidUntilField[logRow] = CaptureLogConsts.UntilMax;
            copyCapturedFields(row, logRow);
            uow.Connection.Insert(logRow);
        }
    }

    /// <inheritdoc/>
    public void OnPrepareQuery(IUndeleteRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public void OnValidateRequest(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public void OnBeforeUndelete(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public void OnAfterUndelete(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public void OnReturn(IUndeleteRequestHandler handler)
    {
    }
}