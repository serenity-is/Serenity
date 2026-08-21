using System.Threading;

namespace Serenity.Web;

/// <summary>
/// Wraps an <see cref="IUserAccessor"/> and adds support for temporary user impersonation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ImpersonatingUserAccessor"/> class
/// that wraps the specified user accessor and adds impersonation support.
/// </remarks>
/// <param name="userContext">The underlying user accessor to delegate to when no impersonation is active.</param>
/// <param name="itemsAccessor">The accessor that provides the per-request item dictionary used to store the impersonation stack.</param>
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
    /// Gets the current user principal, returning the top of the impersonation stack when impersonation is active.
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
    /// Pushes the specified principal onto the impersonation stack.
    /// </summary>
    /// <param name="user">The principal to impersonate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="user"/> is <c>null</c>.</exception>
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
    /// Pops the most recent impersonation from the stack.
    /// </summary>
    /// <exception cref="InvalidOperationException">The impersonation stack is empty.</exception>
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