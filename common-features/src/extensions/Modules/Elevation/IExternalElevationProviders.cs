using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Gives the external login providers that a user can confirm a sensitive action with, instead of the password (see
/// <see cref="IElevationHandler"/>): the ones linked to the account that can prove a new sign-in, e.g. Google. The external
/// login feature registers one, e.g. <c>AddExternalLoginElevation</c> of Pro.
/// </summary>
public interface IExternalElevationProviders
{
    /// <summary>
    /// Gets the providers that a user can confirm a sensitive action with.
    /// </summary>
    /// <param name="user">The user, who is the current user.</param>
    IReadOnlyList<ExternalElevationProvider> GetProviders(ClaimsPrincipal user);
}
