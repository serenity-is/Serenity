namespace Serenity.Services;

/// <summary>
/// Abstract base class for delete request handlers that share state and
/// mode neutral helper methods between synchronous and asynchronous
/// delete request handlers.
/// </summary>
/// <typeparam name="TRow">Entity type</typeparam>
/// <typeparam name="TDeleteRequest">Delete request type</typeparam>
/// <typeparam name="TDeleteResponse">Delete response type</typeparam>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="context">Request context</param>
/// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
public abstract class DeleteRequestHandlerBase<TRow, TDeleteRequest, TDeleteResponse>(IRequestContext context) : IDeleteRequestHandler
    where TRow : class, IRow, IIdRow, new()
    where TDeleteRequest : DeleteRequest
    where TDeleteResponse : DeleteResponse, new()
{

    /// <summary>
    /// Gets the list of delete behaviors.
    /// </summary>
    protected virtual IEnumerable<IDeleteBehavior> GetBehaviors()
    {
        return Context.Behaviors.Resolve<TRow, IDeleteBehavior>(GetType());
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
    public IRequestContext Context { get; private set; } = context ?? throw new ArgumentNullException(nameof(context));

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
    /// Gets the entity being deleted.
    /// </summary>
    public TRow Row { get; protected set; }

    /// <summary>
    /// Gets the request object.
    /// </summary>
    public TDeleteRequest Request { get; protected set; }

    /// <summary>
    /// Gets the response object.
    /// </summary>
    public TDeleteResponse Response { get; protected set; }

    /// <summary>
    /// A state bag for behaviors to preserve state among their methods.
    /// It will be cleared before each request, e.g. Process call.
    /// </summary>
    public IDictionary<string, object> StateBag { get; private set; } = new Dictionary<string, object>();

    IRow IDeleteRequestHandler.Row => Row;
    DeleteRequest IDeleteRequestHandler.Request => Request;
    DeleteResponse IDeleteRequestHandler.Response => Response;
}
