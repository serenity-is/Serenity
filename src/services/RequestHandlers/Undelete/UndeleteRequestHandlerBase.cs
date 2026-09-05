namespace Serenity.Services;

/// <summary>
/// Abstract base class for undelete request handlers that share state and
/// mode neutral helper methods between synchronous and asynchronous
/// undelete request handlers.
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TUndeleteRequest">Undelete request type</typeparam>
/// <typeparam name="TUndeleteResponse">Undelete response type</typeparam>
public abstract class UndeleteRequestHandlerBase<TRow, TUndeleteRequest, TUndeleteResponse> : IUndeleteRequestHandler
    where TRow : class, IRow, IIdRow, new()
    where TUndeleteRequest : UndeleteRequest
    where TUndeleteResponse : UndeleteResponse, new()
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="context">Request context</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
    protected UndeleteRequestHandlerBase(IRequestContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        StateBag = new Dictionary<string, object>();
    }

    /// <summary>
    /// Gets the list of undelete behaviors.
    /// </summary>
    protected virtual IEnumerable<IUndeleteBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IUndeleteBehavior>(GetType());
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
    /// Attaches a cache invalidation call to to OnCommit 
    /// callback of the current unit of work. This would clear
    /// cached items related to this row type.
    /// </summary>
    protected virtual void InvalidateCacheOnCommit()
    {
        Cache.InvalidateOnCommit(UnitOfWork, Row);
    }

    /// <summary>
    /// Gets the two level cache from the request context.
    /// </summary>
    public ITwoLevelCache Cache => Context.Cache;

    /// <summary>
    /// Gets the request context.
    /// </summary>
    public IRequestContext Context { get; private set; }

    /// <summary>
    /// Gets the localizer from the request context.
    /// </summary>
    public ITextLocalizer Localizer => Context.Localizer;

    /// <summary>
    /// Gets the permission service from the request context.
    /// </summary>
    public IPermissionService Permissions => Context.Permissions;

    /// <summary>
    /// Gets the current user from the request context.
    /// </summary>
    public ClaimsPrincipal User => Context.User;

    /// <summary>
    /// Gets the current connection.
    /// </summary>
    public IDbConnection Connection => UnitOfWork.Connection;

    /// <summary>
    /// Gets the current unit of work.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; protected set; }

    /// <summary>
    /// Gets the entity being undeleted.
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Gets the request object.
    /// </summary>
    public TUndeleteRequest Request { get; protected set; }

    /// <summary>
    /// Gets the response object.
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
