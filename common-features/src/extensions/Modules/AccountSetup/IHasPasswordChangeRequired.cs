namespace Serenity.Extensions;

/// <summary>
/// A user definition that tells whether the user must change the password at the next sign-in, e.g. after an administrator
/// set it, see <see cref="PasswordChangeRequirement"/>.
/// </summary>
public interface IHasPasswordChangeRequired
{
    /// <summary>Whether the user must change the password.</summary>
    bool PasswordChangeRequired { get; }
}
