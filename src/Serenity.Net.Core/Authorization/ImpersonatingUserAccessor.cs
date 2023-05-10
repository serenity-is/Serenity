using System.Threading;

namespace Serenity.Web;

/// <summary>
/// Adds impersonation support to any IUserContext implementation
/// </summary>
public class ImpersonatingUserAccessor : IUserAccessor, IImpersonator
{
    private readonly IUserAccessor userContext;
    private readonly IHttpContextItemsAccessor requestContext;
    private readonly ThreadLocal<Stack<ClaimsPrincipal>> impersonationStack = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ImpersonatingUserAccessor"/> class
    /// that wraps passed authorization service and adds impersonation support.
    /// </summary>
    /// <param name="userContext">The user accessor service to wrap with impersonation support.</param>
    /// <param name="itemsAccessor">Request items accessor</param>
    public ImpersonatingUserAccessor(IUserAccessor userContext, IHttpContextItemsAccessor itemsAccessor)
    {
        this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        requestContext = itemsAccessor ?? throw new ArgumentNullException(nameof(itemsAccessor));
    }

    private Stack<ClaimsPrincipal>? GetImpersonationStack(bool createIfNull)
    {
        Stack<ClaimsPrincipal>? stack;
        var requestItems = requestContext.Items;

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
            var impersonationStack = GetImpersonationStack(false);

            if (impersonationStack != null && impersonationStack.Count > 0)
                return impersonationStack.Peek();

            return userContext.User;
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

        var impersonationStack = GetImpersonationStack(true)!;
        impersonationStack.Push(user);
    }

    /// <summary>
    /// Undoes impersonation
    /// </summary>
    /// <exception cref="InvalidOperationException">UndoImpersonate() is called while impersonation stack is empty!</exception>
    public void UndoImpersonate()
    {
        var impersonationStack = GetImpersonationStack(false);
        if (impersonationStack == null || impersonationStack.Count == 0)
            throw new InvalidOperationException("UndoImpersonate() is called while impersonation stack is empty!");

        impersonationStack.Pop();
    }
}