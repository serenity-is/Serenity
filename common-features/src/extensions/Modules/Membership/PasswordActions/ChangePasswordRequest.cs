namespace Serenity.Extensions;

/// <summary>
/// The request model for a change password service.
/// </summary>
[FormScript(LocalTextPrefix = "Forms.Membership.ChangePassword.")]
public class ChangePasswordRequest : ServiceRequest
{
    /// <summary>
    /// The current password of the user.
    /// </summary>
    [PasswordEditor, Required(true), DisplayName("Current Password")]
    public string OldPassword { get; set; }
    /// <summary>
    /// The new password.
    /// </summary>
    [PasswordEditor, Required(true), DisplayName("New Password")]
    public string NewPassword { get; set; }
    /// <summary>
    /// The confirmation of the new password.
    /// </summary>
    [PasswordEditor, Required(true), DisplayName("Confirm Password")]
    public string ConfirmPassword { get; set; }
}