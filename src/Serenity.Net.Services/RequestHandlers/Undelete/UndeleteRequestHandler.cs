namespace Serenity.Services;

/// <summary>
/// Generic base class for undelete request handlers
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TUndeleteRequest">Undelete request type</typeparam>
/// <typeparam name="TUndeleteResponse">Undelete response type</typeparam>
public class UndeleteRequestHandler<TRow, TUndeleteRequest, TUndeleteResponse> : IUndeleteRequestProcessor,
    IUndeleteHandler<TRow, TUndeleteRequest, TUndeleteResponse>
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    /// <summary>
    /// Lazy list of behaviors that is activated for this request
    /// </summary>
    protected Lazy<IUndeleteBehavior[]> behaviors;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException">Context is null</exception>
    public UndeleteRequestHandler(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
        behaviors = new Lazy<IUndeleteBehavior[]>(() => GetBehaviors().ToArray());
    }

    /// <summary>
    /// Gets the list of undelete behaviors
    /// </summary>
    protected virtual IEnumerable<IUndeleteBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IUndeleteBehavior>(GetType());
    }

    /// <summary>
    /// Gets current connection from the unit of work
    /// </summary>
    public IDbConnection Connection => UnitOfWork.Connection;

    /// <summary>
    /// Method that is executed before the actual SQL undelete operation.
    /// </summary>
    protected virtual void OnBeforeUndelete()
    {
        foreach (var behavior in behaviors.Value)
            behavior.OnBeforeUndelete(this);
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
    /// Method that is executed after the actual SQL undelete operation
    /// </summary>
    protected virtual void OnAfterUndelete()
    {
        if (Row is IDisplayOrderRow displayOrderRow)
        {
            var filter = GetDisplayOrderFilter();
            DisplayOrderHelper.ReorderValues(Connection, displayOrderRow, filter,
                Row.IdField.AsObject(Row), displayOrderRow.DisplayOrderField[Row].Value, false);
        }

        foreach (var behavior in behaviors.Value)
            behavior.OnAfterUndelete(this);
    }

    /// <summary>
    /// Validates the parameters of the undelete request.
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
    /// Loads the entity that is going to be undeleted
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
            throw DataValidation.EntityNotFoundError(Row, id, Localizer);
    }

    /// <summary>
    /// Invokes the passed undelete action method
    /// </summary>
    /// <param name="action">Undelete action method</param>
    protected virtual void InvokeUndeleteAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            foreach (var behavior in behaviors.Value.OfType<IUndeleteExceptionBehavior>())
                behavior.OnException(this, exception);

            throw;
        }
    }

    /// <summary>
    /// Executes the actual SQL undelete/update operation
    /// </summary>
    protected virtual void ExecuteUndelete()
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

        InvokeUndeleteAction(() =>
        {
            if (update.Execute(Connection) != 1)
                throw DataValidation.EntityNotFoundError(Row, id, Localizer);
        });

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
    /// Checks that row type implements one of IIsActiveDeletedRow, IIsDeletedRow
    /// or IDeleteLogRow interfaces and it is actual marked as deleted
    /// </summary>
    /// <exception cref="NotImplementedException">Row does not implement any of known interfaces</exception>
    protected virtual bool IsDeleted()
    {
        var isActiveDeletedRow = Row as IIsActiveDeletedRow;
        var isDeletedRow = Row as IIsDeletedRow;
        var deleteLogRow = Row as IDeleteLogRow;

        if (isActiveDeletedRow == null && isDeletedRow == null && deleteLogRow == null)
            throw new NotImplementedException();

        return !((isDeletedRow != null && isDeletedRow.IsDeletedField[Row] != true) ||
                 (isActiveDeletedRow != null && isActiveDeletedRow.IsActiveField[Row] >= 0) ||
                 (deleteLogRow != null && deleteLogRow.DeleteUserIdField.IsNull(Row)));
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
    /// Validates the user permissions for undelete operation
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
    /// Processes the undelete request. This is the entry point for the handler.
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="request">Request</param>
    /// <exception cref="ArgumentNullException">unitofWork is null</exception>
    public TUndeleteResponse Process(IUnitOfWork unitOfWork, TUndeleteRequest request)
    {
        StateBag.Clear();
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException("unitOfWork");
        Request = request;
        Response = new TUndeleteResponse();

        if (request.EntityId == null)
            throw DataValidation.RequiredError("EntityId", Localizer);

        Row = new TRow();

        LoadEntity();
        ValidatePermissions();
        ValidateRequest();

        if (!IsDeleted())
            Response.WasNotDeleted = true;
        else
        {
            OnBeforeUndelete();

            ExecuteUndelete();

            OnAfterUndelete();

            DoAudit();
        }

        OnReturn();

        return Response;
    }

    UndeleteResponse IUndeleteRequestProcessor.Process(IUnitOfWork uow, UndeleteRequest request)
    {
        return Process(uow, (TUndeleteRequest)request);
    }

    /// <inheritdoc/>
    public TUndeleteResponse Undelete(IUnitOfWork uow, TUndeleteRequest request)
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
    public TUndeleteRequest Request { get; protected set; }

    /// <summary>
    /// Response object
    /// </summary>
    public TUndeleteResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; }

    IRow IUndeleteRequestHandler.Row => Row;
    UndeleteRequest IUndeleteRequestHandler.Request => Request;
    UndeleteResponse IUndeleteRequestHandler.Response => Response;
}
