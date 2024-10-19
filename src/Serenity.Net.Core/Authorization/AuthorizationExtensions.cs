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
    /// <param name="userRetriever">User retrieve service</param>
    /// <returns></returns>
    public static TUserDefinition? GetUserDefinition<TUserDefinition>(this ClaimsPrincipal? identity, IUserRetrieveService userRetriever)
        where TUserDefinition : class, IUserDefinition
    {
        if (!IsLoggedIn(identity))
            return null;

        return (TUserDefinition?)userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets user definition for given user identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="userRetriever">User retrieve service</param>
    /// <returns></returns>
    public static IUserDefinition? GetUserDefinition(this ClaimsPrincipal? identity,
        IUserRetrieveService userRetriever)
    {
        if (!IsLoggedIn(identity))
            return null;

        return userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets user definition for given user identity
    /// </summary>
    /// <param name="userRetriever">User retrieve service</param>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static IUserDefinition? GetUserDefinition(this IUserRetrieveService userRetriever, ClaimsPrincipal identity)
    {
        if (!IsLoggedIn(identity))
            return null;

        return userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets user definition for current user
    /// </summary>
    /// <param name="userProvider">User provider</param>
    /// <returns></returns>
    public static IUserDefinition? GetUserDefinition(this IUserProvider userProvider)
    {
        if (!userProvider.IsLoggedIn())
            return null;

        return userProvider.ByUsername(userProvider.User!.Identity.Name);
    }

    /// <summary>
    /// Impersonates a user by its username. Note that this throws an exception if the user is not found,
    /// or the underlying user accessor is not an impersonator.
    /// </summary>
    /// <param name="userProvider">User provider</param>
    /// <param name="username">Username</param>
    /// <param name="authType">Authentication type to use while creating the ClaimsPrincipal. Default is "Impersonation".</param>
    public static void Impersonate(this IUserProvider userProvider, string username, string authType = "Impersonation")
    {
        var principal = userProvider.CreatePrincipal(username, authType);
        userProvider.Impersonate(principal);
    }

    /// <summary>
    /// Tries to invalidate all users in cache if the user retrieve service implements IUserCacheInvalidator.
    /// If not, it tries to expire all users in cache by group name "Default.Users" if cache is not null.
    /// </summary>
    /// <param name="userRetriever">User retrieve service</param>
    /// <param name="cache">Optional cache</param>
    public static void InvalidateAll(this IUserRetrieveService userRetriever, ITwoLevelCache? cache)
    {
        if (userRetriever is IUserCacheInvalidator invalidator)
            invalidator.InvalidateAll();
        else
            cache?.ExpireGroupItems("Default.Users");
    }

    /// <summary>
    /// Tries to invalidate user in cache if the user retrieve service implements IUserCacheInvalidator.
    /// If not, and cache is not null and user is not null, it tries to remove user by id and username from cache.
    /// </summary>
    /// <param name="userRetriever">User retrieve service</param>
    /// <param name="user">User</param>
    /// <param name="cache">Cache</param>
    public static void InvalidateItem(this IUserRetrieveService userRetriever, IUserDefinition? user, ITwoLevelCache? cache)
    {
        if (userRetriever is IUserCacheInvalidator invalidator)
            invalidator.InvalidateItem(user);
        else if (user != null)
        {
            userRetriever.InvalidateById(user.Id, cache);
            userRetriever.InvalidateByUsername(user.Username, cache);
        }
    }

    /// <summary>
    /// Tries to invalidate user by its id if the user retrieve service implements IUserCacheInvalidator.
    /// If not, and cache is not null, it tries to remove user by id from cache.
    /// </summary>
    /// <param name="userRetriever">User retrieve service</param>
    /// <param name="userId">UserId</param>
    /// <param name="cache"></param>
    public static void InvalidateById(this IUserRetrieveService userRetriever, string? userId, ITwoLevelCache? cache)
    {
        if (userRetriever is IUserCacheInvalidator invalidator)
            invalidator.InvalidateById(userId);
        else if (userId != null)
            cache?.Remove("UserByID_" + userId);
    }

    /// <summary>
    /// Tries to invalidate user by its usernae if the user retrieve service implements IUserCacheInvalidator.
    /// If not, and cache is not null, it tries to remove user by id from cache.
    /// </summary>
    /// <param name="userRetriever">User retrieve service</param>
    /// <param name="username">Username</param>
    /// <param name="cache">Cache</param>
    public static void InvalidateByUsername(this IUserRetrieveService userRetriever, string? username, ITwoLevelCache? cache)
    {
        if (userRetriever is IUserCacheInvalidator invalidator)
            invalidator.InvalidateByUsername(username);
        else if (username != null)
            cache?.Remove("UserByName_" + username.ToLowerInvariant());
    }
}