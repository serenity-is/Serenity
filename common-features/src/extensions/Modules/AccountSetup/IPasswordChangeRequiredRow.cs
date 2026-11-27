namespace Serenity.Extensions;

/// <summary>
/// A user row with a field that tells whether the user must change the password at the next sign-in, which the password
/// pages clear when the user changes or resets the password, see <see cref="AccountPasswordActionsPageBase{TUserRow}"/>.
/// </summary>
public interface IPasswordChangeRequiredRow
{
    /// <summary>The field.</summary>
    BooleanField PasswordChangeRequiredField { get; }
}
