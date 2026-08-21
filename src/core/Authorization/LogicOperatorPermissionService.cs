
namespace Serenity.Web;

/// <summary>
/// Decorates an <see cref="IPermissionService"/> to support logical operators (<c>!</c>, <c>&amp;</c>, <c>|</c>, parentheses)
/// in permission expressions.
/// </summary>
/// <remarks>
/// Register this decorator at application startup to enable expressions such as <c>PermissionA &amp; !PermissionB</c>.
/// <code>
/// registrar.RegisterInstance&lt;IPermissionService&gt;(new LogicOperatorPermissionService(new MyPermissionService()))
/// </code>
/// </remarks>
/// <remarks>
/// Creates a new instance of the <see cref="LogicOperatorPermissionService"/> class wrapping the specified permission service.
/// </remarks>
/// <param name="permissionService">The underlying permission service to delegate simple permission checks to.</param>
public class LogicOperatorPermissionService(IPermissionService permissionService) : IPermissionService, ITransientGrantor
{
    private static readonly char[] chars = ['|', '&', '!', '(', ')'];
    private readonly IPermissionService permissionService = permissionService ??
            throw new ArgumentNullException(nameof(permissionService));
    private readonly ConcurrentDictionary<string, string[]> cache = new();

    /// <inheritdoc/>
    public void Grant(params string[] permissions)
    {
        if (permissionService is not ITransientGrantor transientGrantor)
            throw new NotImplementedException();
        transientGrantor.Grant(permissions);
    }

    /// <inheritdoc/>
    public void GrantAll()
    {
        if (permissionService is not ITransientGrantor transientGrantor)
            throw new NotImplementedException();
        transientGrantor.GrantAll();
    }

    /// <summary>
    /// Determines whether the current user has the specified permission or satisfies the given logical permission expression.
    /// </summary>
    /// <param name="permission">The permission key or logical expression (supporting <c>!</c>, <c>&amp;</c>, <c>|</c>, and parentheses).</param>
    /// <returns><c>true</c> if the expression evaluates to granted; otherwise <c>false</c>.</returns>
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrEmpty(permission) ||
            permission.IndexOfAny(chars) < 0)
            return permissionService.HasPermission(permission);

        if (!cache.TryGetValue(permission, out string[] rpnTokens))
        {
            var tokens = PermissionExpressionParser.Tokenize(permission);
            cache[permission] = rpnTokens = PermissionExpressionParser.ShuntingYard(tokens).ToArray();
        }

        return PermissionExpressionParser.Evaluate(rpnTokens, permissionService.HasPermission);
    }

    /// <inheritdoc/>
    public void UndoGrant()
    {
        if (permissionService is not ITransientGrantor transientGrantor)
            throw new NotImplementedException();
        transientGrantor.UndoGrant();
    }

    /// <inheritdoc/>
    public bool IsAllGranted() => permissionService is ITransientGrantor transientGrantor && transientGrantor.IsAllGranted();

    /// <inheritdoc/>
    public IEnumerable<string> GetGranted() => (permissionService as ITransientGrantor)?.GetGranted() ?? [];
}