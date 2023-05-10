using System.Threading;

namespace Serenity.Web;

/// <summary>
/// Adds temporary granting support to any IPermissionService implementation
/// </summary>
/// <remarks>
/// Register this class in your application start, to allow granting permissions temporarily.
/// <code>
/// registrar.RegisterInstance&lt;IPermissionService&gt;(new TransientGrantingPermissionService(new MyPermissionService()))
/// </code>
/// </remarks> 
public class TransientGrantingPermissionService : IPermissionService, ITransientGrantor
{
    private readonly IPermissionService permissionService;
    private readonly IHttpContextItemsAccessor requestContext;
    private readonly ThreadLocal<Stack<HashSet<string>?>> grantingStack = new();

    /// <summary>
    /// Creates a new TransientGrantingPermissionService wrapping passed service
    /// </summary>
    /// <param name="permissionService">Permission service to wrap with transient granting ability</param>
    /// <param name="requestContext">Request context</param>
    public TransientGrantingPermissionService(IPermissionService permissionService, IHttpContextItemsAccessor? requestContext = null)
    {
        this.permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        this.requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    }

    private Stack<HashSet<string>?>? GetGrantingStack(bool createIfNull)
    {
        Stack<HashSet<string>?>? stack;

        var requestItems = requestContext.Items;

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
    /// Checks if user has specified permission
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if user has the permission</returns>
    public bool HasPermission(string permission)
    {
        var grantingStack = GetGrantingStack(false);

        if (grantingStack != null && grantingStack.Count > 0)
        {
            var permissionSet = grantingStack.Peek();
            if (permissionSet == null)
                return true;

            return permissionSet.Contains(permission) ||
                permissionService.HasPermission(permission);
        }

        return permissionService.HasPermission(permission);
    }

    /// <summary>
    /// Grants specified permissions temporarily (or makes it look like)
    /// </summary>
    /// <param name="permissions">List of permission keys</param>
    public void Grant(params string[] permissions)
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

    /// <summary>
    /// Grants all permissions temporarily (or makes it look like)
    /// </summary>
    public void GrantAll()
    {
        var grantingStack = GetGrantingStack(true);
        grantingStack!.Push(null);
    }

    /// <summary>
    /// Undoes last grant or grant all operation
    /// </summary>
    public void UndoGrant()
    {
        var grantingStack = GetGrantingStack(false);
        if (grantingStack == null || grantingStack.Count == 0)
            throw new InvalidOperationException("UndoGrant() is called while Granting stack is empty!");

        grantingStack.Pop();
    }
}