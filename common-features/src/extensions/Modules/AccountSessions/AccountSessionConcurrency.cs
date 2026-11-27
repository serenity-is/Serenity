namespace Serenity.Extensions;

/// <summary>
/// Whether a user can have more than one session at a time, see <see cref="AccountSessionOptions.Concurrent"/>.
/// </summary>
public enum AccountSessionConcurrency
{
    /// <summary>A user can sign in on many devices at once.</summary>
    Allow = 0,

    /// <summary>A sign-in ends the other sessions of the user.</summary>
    SignOutOthers = 1
}
