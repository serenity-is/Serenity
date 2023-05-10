namespace Serenity.Web;

/// <summary>
/// Adds AND OR operator support to any IPermissionService implementation
/// </summary>
/// <remarks>
/// Register this class in your application start, to allow !, |, &amp;, () operators
/// in your permission services, e.g.
/// <code>
/// registrar.RegisterInstance&lt;IPermissionService&gt;(new LogicOperatorPermissionService(new MyPermissionService()))
/// </code>
/// </remarks>
public class LogicOperatorPermissionService : IPermissionService
{
    private static readonly char[] chars = new char[] { '|', '&', '!', '(', ')' };
    private readonly IPermissionService permissionService;
    private readonly ConcurrentDictionary<string, string[]> cache = new();

    /// <summary>
    /// Creates a new LogicOperatorPermissionService wrapping passed IPermissionService
    /// </summary>
    /// <param name="permissionService">Permission service to wrap with AND/OR functionality</param>
    public LogicOperatorPermissionService(IPermissionService permissionService)
    {
        this.permissionService = permissionService ??
            throw new ArgumentNullException(nameof(permissionService));
    }

    /// <summary>
    /// Returns true if user has specified permission
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True if user has specified permission</returns>
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
}