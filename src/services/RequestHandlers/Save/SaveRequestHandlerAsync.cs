namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous save request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TSaveRequest">Save request type</typeparam>
/// <typeparam name="TSaveResponse">Save response type</typeparam>
public class SaveRequestHandlerAsync<TRow, TSaveRequest, TSaveResponse> :
    SaveRequestHandlerBase<TRow, TSaveRequest, TSaveResponse>, ISaveRequestProcessorAsync,
    ISaveHandlerAsync<TRow, TSaveRequest, TSaveResponse>
    where TRow : class, IRow, IIdRow, new()
    where TSaveResponse : SaveResponse, new()
    where TSaveRequest : SaveRequest<TRow>, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<ISaveBehaviorAsync[]> behaviors;

    private bool displayOrderFix;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public SaveRequestHandlerAsync(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<ISaveBehaviorAsync[]>(() =>
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<ISaveBehavior, ISaveBehaviorSync, ISaveBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncSaveBehaviorWrapper(behavior))]);
    }

    /// <summary>
    /// Processes the save request asynchronously. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <param name="requestType">Type of request, Create, Update or Auto</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public async Task<TSaveResponse> ProcessAsync(IUnitOfWork unitOfWork, TSaveRequest request,
        SaveRequestType requestType = SaveRequestType.Auto, CancellationToken cancellationToken = default)
    {
        StateBag.Clear();

        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Request = request ?? throw new ArgumentNullException(nameof(request));

        Response = new TSaveResponse();

        Row = (request.Entity ?? throw new ArgumentNullException(nameof(request.Entity))).Clone();

        if (requestType == SaveRequestType.Auto)
        {
            if (Row.IdField.IsNull(Row))
                requestType = SaveRequestType.Create;
            else
                requestType = SaveRequestType.Update;
        }

        if (requestType == SaveRequestType.Update)
        {
            ValidateAndClearIdField();
            Old = new TRow();
            await LoadOldEntityAsync(cancellationToken).ConfigureAwait(false);
        }
        else
            Old = null;

        await ValidateRequestAsync(cancellationToken).ConfigureAwait(false);
        await SetInternalFieldsAsync(cancellationToken).ConfigureAwait(false);
        await BeforeSaveAsync(cancellationToken).ConfigureAwait(false);

        ClearNonTableAssignments();
        await ExecuteSaveAsync(cancellationToken).ConfigureAwait(false);

        await AfterSaveAsync(cancellationToken).ConfigureAwait(false);

        await PerformAuditingAsync(cancellationToken).ConfigureAwait(false);

        await OnReturnAsync(cancellationToken).ConfigureAwait(false);
        return Response;
    }

    /// <summary>
    /// Called before executing the insert/update statement
    /// </summary>
    protected virtual async Task BeforeSaveAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnBeforeSaveAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Called after executing the insert/update statement
    /// </summary>
    protected virtual async Task AfterSaveAsync(CancellationToken cancellationToken = default)
    {
        await HandleDisplayOrderAsync(afterSave: true, cancellationToken).ConfigureAwait(false);

        foreach (var behavior in behaviors.Value)
            await behavior.OnAfterSaveAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs auditing
    /// </summary>
    protected virtual async Task PerformAuditingAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnAuditAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Invokes the passed save action method
    /// </summary>
    /// <param name="action">Save action method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task InvokeSaveActionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is ISaveExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Executes the actual SQL save operation
    /// </summary>
    protected virtual async Task ExecuteSaveAsync(CancellationToken cancellationToken = default)
    {
        if (IsUpdate)
        {
            if (Row.IsAnyFieldAssigned)
            {
                var idField = Row.IdField;

                if (idField.IndexCompare(Old, Row) != 0)
                {
                    var update = new SqlUpdate(Row.Table);
                    update.Set(Row);
                    update.Where(idField == new ValueCriteria(idField.AsSqlValue(Old)));
                    await InvokeSaveActionAsync(() => update.ExecuteAsync(Connection, ExpectedRows.One,
                        cancellationToken: cancellationToken), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await InvokeSaveActionAsync(() => Connection.UpdateByIdAsync(Row, cancellationToken: cancellationToken),
                        cancellationToken).ConfigureAwait(false);
                }

                Response.EntityId = idField.AsObject(Row);
                InvalidateCacheOnCommit();
            }
        }
        else if (IsCreate)
        {
            var idField = Row.IdField;
            if (idField is not null &&
                idField.Flags.HasFlag(FieldFlags.AutoIncrement))
            {
                await InvokeSaveActionAsync(async () =>
                {
                    var entityId = await Connection.InsertAndGetIDAsync(Row, cancellationToken).ConfigureAwait(false);
                    Response.EntityId = entityId;
                    Row.IdField.AsInvariant(Row, entityId);
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await InvokeSaveActionAsync(() => Connection.InsertAsync(Row, cancellationToken), cancellationToken).ConfigureAwait(false);

                if (idField is not null)
                    Response.EntityId = idField.AsObject(Row);
            }

            InvalidateCacheOnCommit();
        }
    }

    /// <summary>
    /// Handles display order field calculation before and after save
    /// </summary>
    /// <param name="afterSave">True if called after save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task HandleDisplayOrderAsync(bool afterSave, CancellationToken cancellationToken = default)
    {
        if (Row is not IDisplayOrderRow displayOrderRow)
            return;

        if (IsCreate && !afterSave)
        {
            var value = displayOrderRow.DisplayOrderField.AsObject(Row);
            if (value == null || Convert.ToInt32(value) <= 0)
            {
                var filter = GetDisplayOrderFilter();
                displayOrderRow.DisplayOrderField.AsObject(Row,
                    await DisplayOrderHelper.GetNextValueAsync(Connection, displayOrderRow, filter, cancellationToken).ConfigureAwait(false));
            }
            else
                displayOrderFix = true;
        }
        else if (afterSave &&
            ((IsCreate && displayOrderFix) ||
             (IsUpdate && displayOrderRow.DisplayOrderField[Old] != displayOrderRow.DisplayOrderField[Row])))
        {
            await DisplayOrderHelper.ReorderValuesAsync(
                connection: Connection,
                row: displayOrderRow,
                filter: GetDisplayOrderFilter(),
                recordID: Row.IdField.AsObject(Row),
                newDisplayOrder: displayOrderRow.DisplayOrderField[Row].Value,
                hasUniqueConstraint: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Loads the old entity for an update operation
    /// </summary>
    protected virtual async Task LoadOldEntityAsync(CancellationToken cancellationToken = default)
    {
        if (!await (await PrepareQueryAsync(cancellationToken).ConfigureAwait(false)).GetFirstAsync(Connection, cancellationToken).ConfigureAwait(false))
        {
            var idField = Row.IdField;
            var id = Request.EntityId != null ?
                idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
                : idField.AsObject(Row);

            throw DataValidation.EntityNotFoundError(Row, id, Localizer);
        }
    }

    /// <summary>
    /// Called just before the response is returned
    /// </summary>
    protected virtual async Task OnReturnAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnReturnAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Prepares the query for selecting old record in an update operation.
    /// </summary>
    /// <returns>The prepared query.</returns>
    protected virtual async Task<SqlQuery> PrepareQueryAsync(CancellationToken cancellationToken = default)
    {
        var idField = Row.IdField;
        var id = Request.EntityId != null ?
            idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture)
            : idField.AsSqlValue(Row);

        var query = new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Old)
            .SelectTableFields()
            .WhereEqual(idField, id);

        foreach (var behavior in behaviors.Value)
            await behavior.OnPrepareQueryAsync(this, query, cancellationToken).ConfigureAwait(false);

        return query;
    }

    /// <summary>
    /// Sets values for internal fields
    /// </summary>
    protected virtual async Task SetInternalFieldsAsync(CancellationToken cancellationToken = default)
    {
        if (IsCreate)
        {
            await HandleDisplayOrderAsync(afterSave: false, cancellationToken).ConfigureAwait(false);
            SetTrimToEmptyFields();
            SetDefaultValues();
        }

        foreach (var behaviour in behaviors.Value)
            await behaviour.OnSetInternalFieldsAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the request by checking insert / update permissions.
    /// </summary>
    protected virtual async Task ValidateRequestAsync(CancellationToken cancellationToken = default)
    {
        ValidatePermissions();

        var editableFields = ValidateEditable();
        ValidateRequired(editableFields);

        if (IsUpdate)
            ValidateIsActive();

        ValidateFieldValues();

        foreach (var behavior in behaviors.Value)
            await behavior.OnValidateRequestAsync(this, cancellationToken).ConfigureAwait(false);
    }

    async Task<SaveResponse> ISaveRequestProcessorAsync.ProcessAsync(IUnitOfWork uow, ISaveRequest request,
        SaveRequestType type, CancellationToken cancellationToken)
    {
        return await ProcessAsync(uow, (TSaveRequest)request, type, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TSaveResponse> CreateAsync(IUnitOfWork uow, TSaveRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, SaveRequestType.Create, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TSaveResponse> UpdateAsync(IUnitOfWork uow, TSaveRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, SaveRequestType.Update, cancellationToken);
    }
}
