using System.Diagnostics.CodeAnalysis;

namespace Serenity;

/// <summary>
/// Provides helper extension methods for authorization, permission checks, and user retrieval.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Determines whether the user accessed via <paramref name="userAccessor"/> is authenticated.
    /// </summary>
    /// <param name="userAccessor">The accessor that provides the current user principal.</param>
    /// <returns><c>true</c> if a user is authenticated; otherwise <c>false</c>.</returns>
    public static bool IsLoggedIn([NotNullWhen(true)] this IUserAccessor? userAccessor)
    {
        return userAccessor?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Determines whether the specified principal is authenticated.
    /// </summary>
    /// <param name="user">The principal to check.</param>
    /// <returns><c>true</c> if the principal is authenticated; otherwise <c>false</c>.</returns>
    public static bool IsLoggedIn([NotNullWhen(true)] this ClaimsPrincipal? user)
    {
        return user?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Ensures the current user has the specified permission, throwing a validation error with
    /// code <c>AccessDenied</c> otherwise.
    /// </summary>
    /// <param name="permissions">The permission service to query.</param>
    /// <param name="permission">The required permission key.</param>
    /// <param name="localizer">The localizer used to produce the error message.</param>
    /// <exception cref="ArgumentNullException"><paramref name="permissions"/> is <c>null</c>.</exception>
    /// <exception cref="ValidationError">The current user does not have the required permission.</exception>
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
    /// Ensures a user is currently authenticated, throwing a validation error with code <c>NotLoggedIn</c> otherwise.
    /// </summary>
    /// <param name="userAccessor">The accessor that provides the current user.</param>
    /// <param name="localizer">The localizer used to produce the error message.</param>
    /// <exception cref="ValidationError">No user is currently authenticated.</exception>
    public static void ValidateLoggedIn([NotNull] this IUserAccessor? userAccessor, ITextLocalizer? localizer)
    {
        if (!IsLoggedIn(userAccessor))
            throw new ValidationError("NotLoggedIn", null,
                CoreTexts.Authorization.NotLoggedIn.ToString(localizer));
    }

    /// <summary>
    /// Gets the value of the <see cref="ClaimTypes.NameIdentifier"/> claim from the specified principal.
    /// </summary>
    /// <param name="identity">The principal to extract the identifier from.</param>
    /// <returns>The identifier claim value, or <c>null</c> if the principal or claim is not present.</returns>
    public static string? GetIdentifier(this ClaimsPrincipal? identity)
    {
        if (identity == null)
            return null;

        return identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the typed user definition for the specified principal.
    /// </summary>
    /// <typeparam name="TUserDefinition">The concrete user definition type.</typeparam>
    /// <param name="identity">The principal whose identity name is used for lookup.</param>
    /// <param name="userRetriever">The service used to retrieve the user definition.</param>
    /// <returns>The typed user definition, or <c>null</c> if the principal is not authenticated or the user is not found.</returns>
    public static TUserDefinition? GetUserDefinition<TUserDefinition>(this ClaimsPrincipal? identity, IUserRetrieveService userRetriever)
        where TUserDefinition : class, IUserDefinition
    {
        if (!IsLoggedIn(identity))
            return null;

        return (TUserDefinition?)userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets the user definition for the specified principal.
    /// </summary>
    /// <param name="identity">The principal whose identity name is used for lookup.</param>
    /// <param name="userRetriever">The service used to retrieve the user definition.</param>
    /// <returns>The user definition, or <c>null</c> if the principal is not authenticated or the user is not found.</returns>
    public static IUserDefinition? GetUserDefinition(this ClaimsPrincipal? identity,
        IUserRetrieveService userRetriever)
    {
        if (!IsLoggedIn(identity))
            return null;

        return userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets the user definition for the specified principal.
    /// </summary>
    /// <param name="userRetriever">The service used to retrieve the user definition.</param>
    /// <param name="identity">The principal whose identity name is used for lookup.</param>
    /// <returns>The user definition, or <c>null</c> if the principal is not authenticated or the user is not found.</returns>
    public static IUserDefinition? GetUserDefinition(this IUserRetrieveService userRetriever, ClaimsPrincipal identity)
    {
        if (!IsLoggedIn(identity))
            return null;

        return userRetriever.ByUsername(identity.Identity.Name);
    }

    /// <summary>
    /// Gets the user definition for the currently authenticated user.
    /// </summary>
    /// <param name="userProvider">The user provider that supplies the current principal.</param>
    /// <returns>The user definition, or <c>null</c> if no user is authenticated or the user is not found.</returns>
    public static IUserDefinition? GetUserDefinition(this IUserProvider userProvider)
    {
        if (!userProvider.IsLoggedIn())
            return null;

        return userProvider.ByUsername(userProvider.User!.Identity.Name);
    }

    /// <summary>
    /// Impersonates the user with the specified username.
    /// </summary>
    /// <param name="userProvider">The user provider that performs the impersonation.</param>
    /// <param name="username">The username of the user to impersonate.</param>
    /// <param name="authType">The authentication type to assign to the new principal. Defaults to <c>Impersonation</c>.</param>
    /// <exception cref="ArgumentOutOfRangeException">No user exists with the specified username.</exception>
    /// <exception cref="InvalidOperationException">The underlying accessor does not support impersonation.</exception>
    public static void Impersonate(this IUserProvider userProvider, string username, string authType = "Impersonation")
    {
        var principal = userProvider.CreatePrincipal(username, authType);
        userProvider.Impersonate(principal);
    }

    /// <summary>
    /// Removes the specified user from the cache.
    /// </summary>
    /// <remarks>
    /// If <paramref name="userRetriever"/> implements <see cref="IRemoveCachedUser"/>, that mechanism is used.
    /// Otherwise the user is removed from <paramref name="cache"/> by identifier and username when available.
    /// </remarks>
    /// <param name="userRetriever">The user retrieve service.</param>
    /// <param name="user">The user definition to invalidate, or <c>null</c>.</param>
    /// <param name="cache">The two-level cache to remove entries from when the service does not handle invalidation itself.</param>
    public static void RemoveCachedUser(this IUserRetrieveService userRetriever, IUserDefinition? user, ITwoLevelCache? cache)
    {
        RemoveCachedUser(userRetriever, user?.Id, user?.Username, cache);
    }


    /// <summary>
    /// Removes a cached user entry by identifier and/or username.
    /// </summary>
    /// <remarks>
    /// If <paramref name="userRetriever"/> implements <see cref="IRemoveCachedUser"/>, that mechanism is used.
    /// Otherwise entries are removed from <paramref name="cache"/> by the supplied keys.
    /// </remarks>
    /// <param name="userRetriever">The user retrieve service.</param>
    /// <param name="userId">The user identifier, or <c>null</c>.</param>
    /// <param name="username">The username, or <c>null</c>.</param>
    /// <param name="cache">The two-level cache to remove entries from when the service does not handle invalidation itself.</param>
    public static void RemoveCachedUser(this IUserRetrieveService userRetriever, string? userId, string? username, ITwoLevelCache? cache)
    {
        if (userRetriever is IRemoveCachedUser removeCachedUser)
        {
            removeCachedUser.RemoveCachedUser(userId, username);
            return;
        }

        if (userId != null)
            cache?.Remove("UserById_" + userId);

        if (username != null)
            cache?.Remove("UserByName_" + username.ToLowerInvariant());

    }
}