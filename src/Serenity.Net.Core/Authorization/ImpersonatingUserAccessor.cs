using System.Threading;

namespace Serenity.Web;

/// <summary>
/// Adds impersonation support to any IUserContext implementation
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ImpersonatingUserAccessor"/> class
/// that wraps passed authorization service and adds impersonation support.
/// </remarks>
/// <param name="userContext">The user accessor service to wrap with impersonation support.</param>
/// <param name="itemsAccessor">Request items accessor</param>
public class ImpersonatingUserAccessor(IUserAccessor userContext, IHttpContextItemsAccessor itemsAccessor) : IUserAccessor, IImpersonator
{
    private readonly ReaderWriterLockSlim sync = new();
    private readonly IUserAccessor userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    private readonly IHttpContextItemsAccessor? requestContext = itemsAccessor;
    private readonly AsyncLocal<Stack<ClaimsPrincipal>> impersonationStack = new();

    private Stack<ClaimsPrincipal>? GetImpersonationStack(bool createIfNull)
    {
        Stack<ClaimsPrincipal>? stack;
        var requestItems = requestContext?.Items;

        if (requestItems != null)
        {
            stack = requestItems["ImpersonationStack"] as Stack<ClaimsPrincipal>;
            if (stack == null && createIfNull)
                requestItems["ImpersonationStack"] = stack = new Stack<ClaimsPrincipal>();
        }
        else
        {
            stack = impersonationStack.Value;
            if (stack == null && createIfNull)
                impersonationStack.Value = stack = new Stack<ClaimsPrincipal>();
        }

        return stack;
    }

    /// <summary>
    /// Return current user
    /// </summary>
    public ClaimsPrincipal? User
    {
        get
        {
            sync.EnterReadLock();
            try
            {
                var impersonationStack = GetImpersonationStack(false);

                if (impersonationStack != null && impersonationStack.Count > 0)
                    return impersonationStack.Peek();

                return userContext.User;
            }
            finally
            {
                sync.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Temporarily impersonates as a user
    /// </summary>
    /// <param name="user">User to impersonate as</param>
    public void Impersonate(ClaimsPrincipal user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        sync.EnterWriteLock();
        try
        {
            var impersonationStack = GetImpersonationStack(true)!;
            impersonationStack.Push(user);
        }
        finally
        {
            sync.ExitWriteLock();
        }
    }

    /// <summary>
    /// Undoes impersonation
    /// </summary>
    /// <exception cref="InvalidOperationException">UndoImpersonate() is called while impersonation stack is empty!</exception>
    public void UndoImpersonate()
    {
        sync.EnterWriteLock();
        try
        {
            var impersonationStack = GetImpersonationStack(false);
            if (impersonationStack == null || impersonationStack.Count == 0)
                throw new InvalidOperationException("UndoImpersonate() is called while impersonation stack is empty!");

            impersonationStack.Pop();
        }
        finally
        {
            sync.ExitWriteLock();
        }
    }
}