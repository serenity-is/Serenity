namespace Serenity.Services;

/// <summary>
/// Generic base class for delete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TDeleteRequest">Delete request type</typeparam>
/// <typeparam name="TDeleteResponse">Delete response type</typeparam>
public class DeleteRequestHandler<TRow, TDeleteRequest, TDeleteResponse> : IDeleteRequestProcessor,
    IDeleteHandler<TRow, TDeleteRequest, TDeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request
    /// </summary>
    protected Lazy<IDeleteBehavior[]> behaviors;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException">Context is null</exception>
    public DeleteRequestHandler(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
        behaviors = new Lazy<IDeleteBehavior[]>(() => GetBehaviors().ToArray());
    }

    /// <summary>
    /// Gets the list of delete behaviors
    /// </summary>
    protected virtual IEnumerable<IDeleteBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IDeleteBehavior>(GetType());
    }

    /// <summary>
    /// Gets current connection from the unit of work
    /// </summary>
    public IDbConnection Connection => UnitOfWork.Connection;

    /// <summary>
    /// Method that is executed before the actual SQL delete operation.
    /// </summary>
    protected virtual void OnBeforeDelete()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeDelete(this);
    }

    /// <summary>
    /// Gets the display order filter for current group, if the entity 
    /// implements <see cref="IDisplayOrderRow"/> interface
    /// </summary>
    protected virtual BaseCriteria GetDisplayOrderFilter()
    {
        return DisplayOrderFilterHelper.GetDisplayOrderFilterFor(Row);
    }

    /// <summary>
    /// Method that is executed after the actual SQL delete operation
    /// </summary>
    protected virtual void OnAfterDelete()
    {
        if (Row as IDisplayOrderRow != null)
        {
            var filter = GetDisplayOrderFilter();
            DisplayOrderHelper.ReorderValues(Connection, Row as IDisplayOrderRow, filter, -1, 1, false);
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
        var idField = Row.IdField;
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
            foreach (var behavior in behaviors.Value.OfType<IDeleteExceptionBehavior>())
                behavior.OnException(this, exception);

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
        var idField = Row.IdField;
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
                    update.Set(isDeletedRow.IsDeletedField, true);
                }

                if (deleteLogRow != null)
                {
                    update.Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                    deleteLogRow.DeleteDateField.DateTimeKind))
                          .Set(deleteLogRow.DeleteUserIdField, User?.GetIdentifier().TryParseID());
                }
                else if (Row is IUpdateLogRow updateLogRow)
                {
                    update.Set(updateLogRow.UpdateDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
                                    updateLogRow.UpdateDateField.DateTimeKind))
                          .Set(updateLogRow.UpdateUserIdField, User?.GetIdentifier().TryParseID());
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
                    .Set(deleteLogRow.DeleteDateField, DateTimeField.ToDateTimeKind(DateTime.Now,
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
    /// Attaches a cache invalidation call to to OnCommit 
    /// callback of the current unit of work. This would clear
    /// cached items related to this row type.
    /// </summary>
    protected virtual void InvalidateCacheOnCommit()
    {
        Cache.InvalidateOnCommit(UnitOfWork, Row);
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
    /// Validates the user permissions for delete operation
    /// </summary>
    protected virtual void ValidatePermissions()
    {
        var attr = typeof(TRow).GetCustomAttribute<DeletePermissionAttribute>(true) ??
            (PermissionAttributeBase)typeof(TRow).GetCustomAttribute<ModifyPermissionAttribute>(true) ??
            typeof(TRow).GetCustomAttribute<ReadPermissionAttribute>(true);

        if (attr != null)
            Permissions.ValidatePermission(attr.Permission ?? "?", Localizer);
    }

    /// <summary>
    /// Checks if the entity is already deleted
    /// </summary>
    protected virtual bool IsDeleted()
    {
        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;
        var deleteLogRow = Row as IDeleteLogRow;

        return ((isDeletedRow != null && isDeletedRow.IsDeletedField[Row] == true) ||
            (isActiveDeletedRow != null && isActiveDeletedRow.IsActiveField[Row] < 0) ||
            (deleteLogRow != null && !deleteLogRow.DeleteUserIdField.IsNull(Row)));
    }

    /// <summary>
    /// Processes the delete request. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException">unitofWork or request is null</exception>
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

    /// <summary>
    /// Gets the two level cache from the request context
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context
    /// </summary>
    public IRequestContext Context { get; private set; }

    /// <summary>
    /// Gets localizer from the request context
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets permission service from the request context
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets current user from the request context
    /// </summary>
    public ClaimsPrincipal User => Context.User;

    /// <summary>
    /// Gets current unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork { get; protected set; }

    /// <summary>
    /// The entity
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Request object
    /// </summary>
    public TDeleteRequest Request { get; protected set; }

    /// <summary>
    /// Response object
    /// </summary>
    public TDeleteResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; }

    IRow IDeleteRequestHandler.Row => Row;
    DeleteRequest IDeleteRequestHandler.Request => Request;
    DeleteResponse IDeleteRequestHandler.Response => Response;
}