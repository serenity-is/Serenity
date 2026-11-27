using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// The claims of the sessions of the users, see <c>AddAccountSessions</c>.
/// </summary>
public static class AccountSessionClaimTypes
{
    /// <summary>The claim of a sign-in cookie with the ID of its session.</summary>
    public const string SessionId = ClaimTypes.Sid;

    /// <summary>
    /// Gets the ID of the session of a signed-in user, or null.
    /// </summary>
    /// <param name="user">The user.</param>
    public static string? GetSessionId(this ClaimsPrincipal? user) =>
        user?.Identity?.IsAuthenticated == true ? user.FindFirst(SessionId)?.Value : null;
}
