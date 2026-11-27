namespace Serenity.Extensions;

/// <summary>
/// Revokes the tokens of a user that are not sign-in cookies, e.g. the refresh tokens of an OpenID server, when the
/// sessions of the user are ended for security, e.g. after a password change, see <see cref="IAccountSessionManager"/>.
/// </summary>
public interface IAccountTokenRevoker
{
    /// <summary>
    /// Revokes all the tokens of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    void RevokeAll(string userId);
}
