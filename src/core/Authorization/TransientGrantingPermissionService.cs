namespace Serenity.Web;

/// <summary>
/// Decorates an <see cref="IPermissionService"/> to support temporarily granting permissions.
/// </summary>
/// <remarks>
/// Register this decorator at application startup to enable temporary permission grants.
/// <code>
/// registrar.RegisterInstance&lt;IPermissionService&gt;(new TransientGrantingPermissionService(new MyPermissionService()))
/// </code>
/// </remarks>
/// <remarks>
/// Creates a new instance of the <see cref="TransientGrantingPermissionService"/> class wrapping the specified service.
/// </remarks>
/// <param name="permissionService">The underlying permission service to delegate to when no transient grant is active.</param>
/// <param name="requestContext">The accessor that provides per-request storage for the granting stack.</param>
public class TransientGrantingPermissionService(IPermissionService? permissionService = null, IHttpContextItemsAccessor? requestContext = null) : IPermissionService, ITransientGrantor
{
    private readonly ReaderWriterLockSlim sync = new();
    private readonly IHttpContextItemsAccessor? requestContext = requestContext;
    private readonly AsyncLocal<Stack<HashSet<string>?>> grantingStack = new();
    
    private Stack<HashSet<string>?>? GetGrantingStack(bool createIfNull)
    {
        Stack<HashSet<string>?>? stack;

        var requestItems = requestContext?.Items;

        if (requestItems != null)
        {
            stack = requestItems["GrantingStack"] as Stack<HashSet<string>?>;
            if (stack == null && createIfNull)
                requestItems["GrantingStack"] = stack = new Stack<HashSet<string>?>();
        }
        else
        {
            stack = grantingStack.Value;
            if (stack == null && createIfNull)
                grantingStack.Value = stack = new Stack<HashSet<string>?>();
        }

        return stack;
    }

    /// <summary>
    /// Determines whether the current user has the specified permission, taking transient grants into account.
    /// </summary>
    /// <param name="permission">The permission key to check.</param>
    /// <returns><c>true</c> if the permission is granted; otherwise <c>false</c>.</returns>
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrEmpty(permission))
            return false;

        sync.EnterReadLock();
        try
        {
            var grantingStack = GetGrantingStack(false);

            if (grantingStack != null && grantingStack.Count > 0)
            {
                var permissionSet = grantingStack.Peek();
                if (permissionSet == null)
                    return true;

                return permissionSet.Contains(permission) ||
                    (permissionService != null && permissionService.HasPermission(permission));
            }

            return permissionService != null && permissionService.HasPermission(permission);
        }
        finally
        {
            sync.ExitReadLock();
        }
    }

    /// <summary>
    /// Temporarily grants the specified permissions.
    /// </summary>
    /// <param name="permissions">The permission keys to grant.</param>
    /// <exception cref="ArgumentNullException"><paramref name="permissions"/> is <c>null</c> or empty.</exception>
    public void Grant(params string[] permissions)
    {
        sync.EnterWriteLock();
        try
        {
            if (permissions == null || permissions.Length == 0)
                throw new ArgumentNullException("permissions");

            var grantingStack = GetGrantingStack(true);

            if (grantingStack!.Count > 0)
            {
                var oldSet = grantingStack.Peek();
                if (oldSet == null)
                    grantingStack.Push(null);
                else
                {
                    var newSet = new HashSet<string>(oldSet);
                    newSet.AddRange(permissions);
                    grantingStack.Push(newSet);
                }
            }
            else
            {
                grantingStack.Push(new HashSet<string>(permissions));
            }
        }
        finally
        {
            sync.ExitWriteLock();
        }
    }

    /// <summary>
    /// Temporarily grants all permissions.
    /// </summary>
    public void GrantAll()
    {
        sync.EnterWriteLock();
        try
        {
            var grantingStack = GetGrantingStack(true);
            grantingStack!.Push(null);
        }
        finally
        {
            sync.ExitWriteLock();
        }
    }

    /// <summary>
    /// Reverts the most recent <see cref="Grant"/> or <see cref="GrantAll"/> operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">The granting stack is empty.</exception>
    public void UndoGrant()
    {
        sync.EnterWriteLock();
        try
        {
            var grantingStack = GetGrantingStack(false);
            if (grantingStack == null || grantingStack.Count == 0)
                throw new InvalidOperationException("UndoGrant() is called while Granting stack is empty!");

            grantingStack.Pop();
        }
        finally
        {
            sync.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public bool IsAllGranted()
    {
        sync.EnterReadLock();
        try
        {
            var grantingStack = GetGrantingStack(false);

            if (grantingStack != null && grantingStack.Count > 0)
                return grantingStack.Peek() == null;
        }
        finally
        {
            sync.ExitReadLock();
        }

        return false;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetGranted()
    {
        sync.EnterReadLock();
        try
        {
            var grantingStack = GetGrantingStack(false);

            if (grantingStack != null && grantingStack.Count > 0)
            {
                var permissionSet = grantingStack.Peek();
                if (permissionSet != null)
                    return permissionSet;
            }
        }
        finally
        {
            sync.ExitReadLock();
        }

        return [];
    }

}