using System.Diagnostics.CodeAnalysis;

namespace Serenity;

/// <summary>
/// Authorization extension methods
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Returns true if user is logged in (authenticated).
    /// </summary>
    public static bool IsLoggedIn([NotNullWhen(true)] this IUserAccessor? userAccessor)
    {
        return userAccessor?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Returns true if user is logged in (authenticated).
    /// </summary>
    public static bool IsLoggedIn([NotNullWhen(true)] this ClaimsPrincipal? user)
    {
        return user?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Checks if current user has given permission and throws a validation error with 
    /// "AccessDenied" error code if not.
    /// </summary>
    /// <param name="permissions">Permissions service</param>
    /// <param name="permission">Permission key</param>
    /// <param name="localizer">Localizer</param>
    public static void ValidatePermission(this IPermissionService permissions,
        string permission, ITextLocalizer localizer)
    {
        if (permissions == null)
            throw new ArgumentNullException(nameof(permissions));

        if (!permissions.HasPermission(permission))
            throw new ValidationError("AccessDenied", null,
                CoreTexts.Authorization.AccessDenied.ToString(localizer));
    }

    /// <summary>
    /// Checks if there is a currently logged user and throws a validation error with
    /// "NotLoggedIn" error code if not.
    /// </summary>
    public static void ValidateLoggedIn([NotNull] this IUserAccessor? userAccessor, ITextLocalizer? localizer)
    {
        if (!IsLoggedIn(userAccessor))
            throw new ValidationError("NotLoggedIn", null,
                CoreTexts.Authorization.NotLoggedIn.ToString(localizer));
    }

    /// <summary>
    /// Gets name identifier claim from given identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static string? GetIdentifier(this ClaimsPrincipal? identity)
    {
        if (identity == null)
            return null;

        return identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets user definition for given user identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="userRetrieveService">User retrieve service</param>
    /// <returns></returns>
    public static TUserDefinition? GetUserDefinition<TUserDefinition>(this ClaimsPrincipal? identity,
        IUserRetrieveService userRetrieveService)
            where TUserDefinition : class, IUserDefinition
    {
        if (!IsLoggedIn(identity))
            return null;

        return (TUserDefinition)userRetrieveService.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets user definition for given user identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="userRetrieveService">User retrieve service</param>
    /// <returns></returns>
    public static IUserDefinition? GetUserDefinition(this ClaimsPrincipal? identity,
        IUserRetrieveService userRetrieveService)
    {
        if (!IsLoggedIn(identity))
            return null;

        return userRetrieveService.ByUsername(identity.Identity.Name);
    }

}