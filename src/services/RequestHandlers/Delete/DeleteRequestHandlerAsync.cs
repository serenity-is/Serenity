namespace Serenity.Services;

/// <summary>
/// Generic base class for asynchronous delete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TDeleteRequest">Delete request type</typeparam>
/// <typeparam name="TDeleteResponse">Delete response type</typeparam>
public class DeleteRequestHandlerAsync<TRow, TDeleteRequest, TDeleteResponse> :
    DeleteRequestHandlerBase<TRow, TDeleteRequest, TDeleteResponse>, IDeleteRequestProcessorAsync,
    IDeleteHandlerAsync<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IDeleteBehaviorAsync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public DeleteRequestHandlerAsync(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IDeleteBehaviorAsync[]>(() =>
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<IDeleteBehavior, IDeleteBehaviorSync, IDeleteBehaviorAsync>(
                GetBehaviors(), behavior => new SyncToAsyncDeleteBehaviorWrapper(behavior))]);
    }

    /// <summary>
    /// Method that is executed before the actual SQL delete operation.
    /// </summary>
    protected virtual async Task OnBeforeDeleteAsync(CancellationToken cancellationToken = default)
    {
        foreach (var behavior in behaviors.Value)
            await behavior.OnBeforeDeleteAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Method that is executed after the actual SQL delete operation
    /// </summary>
    protected virtual async Task OnAfterDeleteAsync(CancellationToken cancellationToken = default)
    {
        if (Row is IDisplayOrderRow displayOrderRow)
        {
            var filter = GetDisplayOrderFilter();
            await DisplayOrderHelper.ReorderValuesAsync(Connection, displayOrderRow, filter, -1, 1,
                hasUniqueConstraint: false, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        foreach (var behavior in behaviors.Value)
            await behavior.OnAfterDeleteAsync(this, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Validates the parameters of the delete request.
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
    /// Loads the entity that is going to be deleted
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
            throw DataValidation.EntityNotFoundError(Row, Request.EntityId, Localizer);
    }

    /// <summary>
    /// Invokes the passed delete action method
    /// </summary>
    /// <param name="action">Delete action method</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected virtual async Task InvokeDeleteActionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value)
            {
                if (((behavior as IWrappedBehavior)?.WrappedBehavior ?? behavior) is IDeleteExceptionBehavior exceptionBehavior)
                    exceptionBehavior.OnException(this, exception);
            }

            throw;
        }
    }

    /// <summary>
    /// Executes the actual SQL delete operation
    /// </summary>
    protected virtual async Task ExecuteDeleteAsync(CancellationToken cancellationToken = default)
    {
        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;
        var deleteLogRow = Row as IDeleteLogRow;
        var idField = Row.IdField;
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        if (isActiveDeletedRow == null && isDeletedRow == null && deleteLogRow == null)
        {
            var delete = new SqlDelete(Row.Table)
                .WhereEqual(idField, id);

            await InvokeDeleteActionAsync(async () =>
            {
                if (await delete.ExecuteAsync(Connection, cancellationToken: cancellationToken).ConfigureAwait(false) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id, Localizer);
            }, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            if (isDeletedRow != null || isActiveDeletedRow != null)
            {
                var update = new SqlUpdate(Row.Table)
                    .WhereEqual(idField, id)
                    .Where(ServiceQueryHelper.GetNotDeletedCriteria(Row));

                if (isActiveDeletedRow != null)
                {
                    update.Set(isActiveDeletedRow.IsActiveField, -1);
                }
                else
                {
                    update.Set(isDeletedRow.IsDeletedField, true);
                }

                if (deleteLogRow != null)
                {
                    update.Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                    deleteLogRow.DeleteDateField.DateTimeKind))
                          .Set(deleteLogRow.DeleteUserIdField, User?.GetIdentifier().TryParseID());
                }
                else
                {
                    if (Row is IUpdateLogRow updateLogRow)
                    {
                        update.Set(updateLogRow.UpdateDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                        updateLogRow.UpdateDateField.DateTimeKind))
                                .Set(updateLogRow.UpdateUserIdField, User?.GetIdentifier().TryParseID());
                    }
                }

                await InvokeDeleteActionAsync(async () =>
                {
                    if (await update.ExecuteAsync(Connection, ExpectedRows.One, cancellationToken: cancellationToken).ConfigureAwait(false) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id, Localizer);
                }, cancellationToken).ConfigureAwait(false);
            }
            else //if (deleteLogRow != null)
            {
                var update = new SqlUpdate(Row.Table)
                    .Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                deleteLogRow.DeleteDateField.DateTimeKind))
                    .Set(deleteLogRow.DeleteUserIdField, User?.GetIdentifier().TryParseID())
                    .WhereEqual(idField, id)
                    .Where(new Criteria(deleteLogRow.DeleteUserIdField).IsNull());

                await InvokeDeleteActionAsync(async () =>
                {
                    if (await update.ExecuteAsync(Connection, ExpectedRows.One, cancellationToken: cancellationToken).ConfigureAwait(false) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id, Localizer);
                }, cancellationToken).ConfigureAwait(false);
            }
        }

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
    /// Processes the delete request asynchronously. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public async Task<TDeleteResponse> ProcessAsync(IUnitOfWork unitOfWork, TDeleteRequest request,
        CancellationToken cancellationToken = default)
    {
        StateBag.Clear();
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Response = new TDeleteResponse();

        if (request.EntityId == null)
            throw DataValidation.RequiredError(nameof(request.EntityId), Localizer);

        Row = new TRow();

        await LoadEntityAsync(cancellationToken).ConfigureAwait(false);
        ValidatePermissions();
        await ValidateRequestAsync(cancellationToken).ConfigureAwait(false);

        if (IsDeleted())
            Response.WasAlreadyDeleted = true;
        else
        {
            await OnBeforeDeleteAsync(cancellationToken).ConfigureAwait(false);

            await ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

            await OnAfterDeleteAsync(cancellationToken).ConfigureAwait(false);

            await DoAuditAsync(cancellationToken).ConfigureAwait(false);
        }

        await OnReturnAsync(cancellationToken).ConfigureAwait(false);

        return Response;
    }

    async Task<DeleteResponse> IDeleteRequestProcessorAsync.ProcessAsync(IUnitOfWork uow, DeleteRequest request,
        CancellationToken cancellationToken)
    {
        return await ProcessAsync(uow, (TDeleteRequest)request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<TDeleteResponse> DeleteAsync(IUnitOfWork uow, TDeleteRequest request, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(uow, request, cancellationToken);
    }
}
