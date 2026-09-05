namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous undelete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TUndeleteRequest">Undelete request type</typeparam>
/// <typeparam name="TUndeleteResponse">Undelete response type</typeparam>
public class UndeleteRequestHandlerAsync<TRow, TUndeleteRequest, TUndeleteResponse> :
    UndeleteRequestHandlerBase<TRow, TUndeleteRequest, TUndeleteResponse>, IUndeleteRequestProcessorAsync,
    IUndeleteHandlerAsync<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IUndeleteBehaviorAsync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public UndeleteRequestHandlerAsync(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IUndeleteBehaviorAsync[]>(() =>
            BehaviorProviderExtensions.AutoWrapBehaviors<IUndeleteBehavior, IUndeleteBehaviorSync, IUndeleteBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncUndeleteBehaviorWrapper(behavior)).ToArray());
    }

    /// <summary>
    /// Method that is executed before the actual SQL undelete operation.
    /// </summary>
    protected virtual async Task OnBeforeUndeleteAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnBeforeUndeleteAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Method that is executed after the actual SQL undelete operation
    /// </summary>
    protected virtual async Task OnAfterUndeleteAsync(CancellationToken cancellationToken = default)
    {
        if (Row is IDisplayOrderRow displayOrderRow)
        {
            var filter = GetDisplayOrderFilter();
            await DisplayOrderHelper.ReorderValuesAsync(Connection, displayOrderRow, filter,
                Row.IdField.AsObject(Row), displayOrderRow.DisplayOrderField[Row].Value,
                hasUniqueConstraint: false, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        foreach (var behavior in behaviors.Value)
            await behavior.OnAfterUndeleteAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the parameters of the undelete request.
    /// </summary>
    protected virtual async Task ValidateRequestAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnValidateRequestAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Prepares the query used to select the existing record
    /// </summary>
    /// <param name="query">The query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task PrepareQueryAsync(SqlQuery query, CancellationToken cancellationToken = default)
    {
        query.SelectTableFields();

        foreach (var behavior in behaviors.Value)
            await behavior.OnPrepareQueryAsync(this, query, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Loads the entity that is going to be undeleted
    /// </summary>
    protected virtual async Task LoadEntityAsync(CancellationToken cancellationToken = default)
    {
        var idField = Row.IdField;
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        var query = new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Row)
            .WhereEqual(idField, id);

        await PrepareQueryAsync(query, cancellationToken).ConfigureAwait(false);

        if (!await query.GetFirstAsync(Connection, cancellationToken).ConfigureAwait(false))
            throw DataValidation.EntityNotFoundError(Row, id, Localizer);
    }

    /// <summary>
    /// Invokes the passed undelete action method
    /// </summary>
    /// <param name="action">Undelete action method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task InvokeUndeleteActionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IUndeleteExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Executes the actual SQL undelete/update operation
    /// </summary>
    protected virtual async Task ExecuteUndeleteAsync(CancellationToken cancellationToken = default)
    {
        var idField = Row.IdField;
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;

        var update = new SqlUpdate(Row.Table)
            .WhereEqual(idField, id);

        if (isActiveDeletedRow != null)
        {
            update.Set(isActiveDeletedRow.IsActiveField, 1)
                .WhereEqual(isActiveDeletedRow.IsActiveField, -1);
        }
        else if (isDeletedRow != null)
        {
            update.Set(isDeletedRow.IsDeletedField, false)
                .WhereEqual(isDeletedRow.IsDeletedField, 1);
        }

        if (Row is IDeleteLogRow deleteLogRow)
        {
            update.Set(deleteLogRow.DeleteUserIdField, null)
                .Set(deleteLogRow.DeleteDateField, null);

            if (isActiveDeletedRow == null && isDeletedRow == null)
                update.Where(deleteLogRow.DeleteUserIdField.IsNotNull());
        }

        await InvokeUndeleteActionAsync(async () =>
        {
            if (await update.ExecuteAsync(Connection, ExpectedRows.One, cancellationToken: cancellationToken).ConfigureAwait(false) != 1)
                throw DataValidation.EntityNotFoundError(Row, id, Localizer);
        }, cancellationToken).ConfigureAwait(false);

        InvalidateCacheOnCommit();
    }

    /// <summary>
    /// Performs auditing
    /// </summary>
    protected virtual async Task DoAuditAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnAuditAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// The method that is called just before the response is returned.
    /// </summary>
    protected virtual async Task OnReturnAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnReturnAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the undelete request asynchronously. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> is <c>null</c>.</exception>
    public async Task<TUndeleteResponse> ProcessAsync(IUnitOfWork unitOfWork, TUndeleteRequest request,
        CancellationToken cancellationToken = default)
    {
        StateBag.Clear();
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Response = new TUndeleteResponse();

        if (request.EntityId == null)
            throw DataValidation.RequiredError(nameof(request.EntityId), Localizer);

        Row = new TRow();

        await LoadEntityAsync(cancellationToken).ConfigureAwait(false);
        ValidatePermissions();
        await ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        if (!IsDeleted())
            Response.WasNotDeleted = true;
        else
        {
            await OnBeforeUndeleteAsync(cancellationToken).ConfigureAwait(false);

            await ExecuteUndeleteAsync(cancellationToken).ConfigureAwait(false);

            await OnAfterUndeleteAsync(cancellationToken).ConfigureAwait(false);

            await DoAuditAsync(cancellationToken).ConfigureAwait(false);
        }

        await OnReturnAsync(cancellationToken).ConfigureAwait(false);

        return Response;
    }

    async Task<UndeleteResponse> IUndeleteRequestProcessorAsync.ProcessAsync(IUnitOfWork uow, UndeleteRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessAsync(uow, (TUndeleteRequest)request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TUndeleteResponse> UndeleteAsync(IUnitOfWork uow, TUndeleteRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, cancellationToken);
    }
}
