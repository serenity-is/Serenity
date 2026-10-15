namespace Serenity.Extensions;

/// <summary>
/// The request model for a reset password service.
/// </summary>
[FormScript(LocalTextPrefix = "Forms.Membership.ResetPassword.")]
public class ResetPasswordRequest : ServiceRequest
{
    /// <summary>
    /// The reset token issued when the reset password email was sent.
    /// </summary>
    [IgnoreUIField]
    public string? Token { get; set; }
    /// <summary>
    /// The new password.
    /// </summary>
    [PasswordEditor, Required(true), DisplayText("New Password"), Placeholder("password")]
    public string? NewPassword { get; set; }
    /// <summary>
    /// The confirmation of the new password.
    /// </summary>
    [PasswordEditor, Required(true), DisplayText("Confirm Password"), Placeholder("confirm password")]
    public string? ConfirmPassword { get; set; }
}