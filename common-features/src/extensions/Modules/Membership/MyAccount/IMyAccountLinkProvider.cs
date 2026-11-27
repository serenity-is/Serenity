using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Gives the links of the my account page for what a feature lets the users manage, e.g. the password, the two-factor
/// authentication, or the external logins. Register one with
/// <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;IMyAccountLinkProvider, MyProvider&gt;())</c>.
/// </summary>
public interface IMyAccountLinkProvider
{
    /// <summary>
    /// Gets the links for the signed-in user, if any.
    /// </summary>
    /// <param name="user">The signed-in user.</param>
    IEnumerable<MyAccountLink> GetLinks(ClaimsPrincipal user);
}
