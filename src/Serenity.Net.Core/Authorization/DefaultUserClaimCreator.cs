using System.Security.Principal;

namespace Serenity.Services;

/// <summary>
/// Default implementation for IUserClaimCreator
/// </summary>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="userRetrieveService"></param>
/// <exception cref="ArgumentNullException"></exception>
public class DefaultUserClaimCreator(IUserRetrieveService userRetrieveService) : IUserClaimCreator
{
    private readonly IUserRetrieveService userRetrieveService = userRetrieveService ?? throw new ArgumentNullException(nameof(userRetrieveService));

    /// <summary>
    /// Add User Claims To Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="userDefinition"></param>
    protected virtual void AddClaims(ClaimsIdentity identity, IUserDefinition userDefinition)
    {
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userDefinition.Id));
    }

    /// <summary>
    /// Create user Principal
    /// </summary>
    /// <param name="username"></param>
    /// <param name="authType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public virtual ClaimsPrincipal CreatePrincipal(string username, string authType)
    {
        if (username is null)
            throw new ArgumentNullException(nameof(username));

        var user = userRetrieveService.ByUsername(username) ?? 
            throw new ArgumentOutOfRangeException(nameof(username));
        if (authType == null)
            throw new ArgumentNullException(nameof(authType));

        var identity = new GenericIdentity(username, authType);
        AddClaims(identity, user);

        return new ClaimsPrincipal(identity);
        
    }
}