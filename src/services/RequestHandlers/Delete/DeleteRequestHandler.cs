namespace Serenity.Services;

/// <summary>
/// Generic base class for delete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TDeleteRequest">Delete request type</typeparam>
/// <typeparam name="TDeleteResponse">Delete response type</typeparam>
public class DeleteRequestHandler<TRow, TDeleteRequest, TDeleteResponse> :
    DeleteRequestHandlerBase<TRow, TDeleteRequest, TDeleteResponse>, IDeleteRequestProcessor,
    IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request.
    /// </summary>
    protected Lazy<IDeleteBehaviorSync[]> behaviors;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    public DeleteRequestHandler(IRequestContext context) : base(context)
    {
        behaviors = new Lazy<IDeleteBehaviorSync[]>(() =>
            [.. BehaviorProviderExtensions.AutoWrapBehaviors<IDeleteBehavior, IDeleteBehaviorAsync, IDeleteBehaviorSync>(
                GetBehaviors(), behavior => new AsyncToSyncDeleteBehaviorWrapper(behavior))]);
    }

    /// <summary>
    /// Method that is executed before the actual SQL delete operation.
    /// </summary>
    protected virtual void OnBeforeDelete()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeDelete(this);
    }

    /// <summary>
    /// Method that is executed after the actual SQL delete operation
    /// </summary>
    protected virtual void OnAfterDelete()
    {
        if (Row is IDisplayOrderRow displayOrderRow)
        {
            var filter = GetDisplayOrderFilter();
            DisplayOrderHelper.ReorderValues(Connection, displayOrderRow, filter, -1, 1, false);
        }

        foreach (var behavior in behaviors.Value)
            behavior.OnAfterDelete(this);
    }

    /// <summary>
    /// Validates the parameters of the delete request.
    /// </summary>
    protected virtual void ValidateRequest()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnValidateRequest(this);
    }

    /// <summary>
    /// Prepares the query used to select the existing record
    /// </summary>
    /// <param name="query">The query</param>
    protected virtual void PrepareQuery(SqlQuery query)
    {
        query.SelectTableFields();

        foreach (var behavior in behaviors.Value)
            behavior.OnPrepareQuery(this, query);
    }

    /// <summary>
    /// Loads the entity that is going to be deleted
    /// </summary>
    protected virtual void LoadEntity()
    {
        var idField = Row.GetIdField();
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        var query = new SqlQuery()
            .Dialect(Connection.GetDialect())
            .From(Row)
            .WhereEqual(idField, id);

        PrepareQuery(query);

        if (!query.GetFirst(Connection))
            throw DataValidation.EntityNotFoundError(Row, Request.EntityId, Localizer);
    }

    /// <summary>
    /// Invokes the passed delete action method
    /// </summary>
    /// <param name="action">Delete action method</param>
    protected virtual void InvokeDeleteAction(Action action)
    {
        try
        {
            action();
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
    protected virtual void ExecuteDelete()
    {
        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;
        var deleteLogRow = Row as IDeleteLogRow;
        var idField = Row.GetIdField();
        var id = idField.ConvertValue(Request.EntityId, CultureInfo.InvariantCulture);

        if (isActiveDeletedRow == null && isDeletedRow == null && deleteLogRow == null)
        {
            var delete = new SqlDelete(Row.Table)
                .WhereEqual(idField, id);

            InvokeDeleteAction(() =>
            {
                if (delete.Execute(Connection) != 1)
                    throw DataValidation.EntityNotFoundError(Row, id, Localizer);
            });
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
                    update.Set(isDeletedRow!.IsDeletedField, true);
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

                InvokeDeleteAction(() =>
                {
                    if (update.Execute(Connection) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id, Localizer);
                });
            }
            else //if (deleteLogRow != null)
            {
                var update = new SqlUpdate(Row.Table)
                    .Set(deleteLogRow!.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                deleteLogRow.DeleteDateField.DateTimeKind))
                    .Set(deleteLogRow.DeleteUserIdField, User?.GetIdentifier().TryParseID())
                    .WhereEqual(idField, id)
                    .Where(new Criteria(deleteLogRow.DeleteUserIdField).IsNull());

                InvokeDeleteAction(() =>
                {
                    if (update.Execute(Connection) != 1)
                        throw DataValidation.EntityNotFoundError(Row, id, Localizer);
                });
            }
        }

        InvalidateCacheOnCommit();
    }

    /// <summary>
    /// Performs auditing
    /// </summary>
    protected virtual void DoAudit()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnAudit(this);
    }

    /// <summary>
    /// The method that is called just before the response is returned.
    /// </summary>
    protected virtual void OnReturn()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnReturn(this);
    }

    /// <summary>
    /// Processes the delete request. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException"><paramref name="unitOfWork"/> or <paramref name="request"/> is <c>null</c>.</exception>
    public TDeleteResponse Process(IUnitOfWork unitOfWork, TDeleteRequest request)
    {
        StateBag.Clear();
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Request = request ?? throw new ArgumentNullException(nameof(request));
        Response = new TDeleteResponse();

        if (request.EntityId == null)
            throw DataValidation.RequiredError(nameof(request.EntityId), Localizer);

        Row = new TRow();

        LoadEntity();
        ValidatePermissions();
        ValidateRequest();

        if (IsDeleted())
            Response.WasAlreadyDeleted = true;
        else
        {
            OnBeforeDelete();

            ExecuteDelete();

            OnAfterDelete();

            DoAudit();
        }

        OnReturn();

        return Response;
    }

    DeleteResponse IDeleteRequestProcessor.Process(IUnitOfWork uow, DeleteRequest request)
    {
        return Process(uow, (TDeleteRequest)request);
    }

    /// <inheritdoc/>
    public TDeleteResponse Delete(IUnitOfWork uow, TDeleteRequest request)
    {
        return Process(uow, request);
    }
}
