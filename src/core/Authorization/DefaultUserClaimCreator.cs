using System.Security.Principal;

namespace Serenity.Services;

/// <summary>
/// Default implementation of <see cref="IUserClaimCreator"/> that builds a <see cref="ClaimsPrincipal"/>
/// from a user definition retrieved via <see cref="IUserRetrieveService"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="DefaultUserClaimCreator"/> class.
/// </remarks>
/// <param name="userRetriever">The service used to retrieve user definitions by username.</param>
/// <exception cref="ArgumentNullException"><paramref name="userRetriever"/> is <c>null</c>.</exception>
public class DefaultUserClaimCreator(IUserRetrieveService userRetriever) : IUserClaimCreator
{
    private readonly IUserRetrieveService userRetriever = userRetriever ?? throw new ArgumentNullException(nameof(userRetriever));

    /// <summary>
    /// Adds claims to the specified identity for the given user definition.
    /// </summary>
    /// <param name="identity">The identity to add claims to.</param>
    /// <param name="userDefinition">The user definition that provides claim values.</param>
    protected virtual void AddClaims(ClaimsIdentity identity, IUserDefinition userDefinition)
    {
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userDefinition.Id));
    }

    /// <summary>
    /// Creates a principal for the specified user.
    /// </summary>
    /// <param name="username">The username of the user to create the principal for.</param>
    /// <param name="authType">The authentication type to assign to the created identity.</param>
    /// <returns>The created <see cref="ClaimsPrincipal"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="username"/> or <paramref name="authType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">No user exists with the specified <paramref name="username"/>.</exception>
    public virtual ClaimsPrincipal CreatePrincipal(string username, string authType)
    {
        if (username is null)
            throw new ArgumentNullException(nameof(username));

        var user = userRetriever.ByUsername(username) ?? 
            throw new ArgumentOutOfRangeException(nameof(username));
        if (authType == null)
            throw new ArgumentNullException(nameof(authType));

        var identity = new GenericIdentity(username, authType);
        AddClaims(identity, user);

        return new ClaimsPrincipal(identity);
        
    }
}