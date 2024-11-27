namespace Serenity.Extensions;

[FormScript(LocalTextPrefix = "Forms.Membership.ChangePassword.")]
public class ChangePasswordRequest : ServiceRequest
{
    [PasswordEditor, Required(true), DisplayName("Current Password")]
    public string OldPassword { get; set; }
    [PasswordEditor, Required(true), DisplayName("New Password")]
    public string NewPassword { get; set; }
    [PasswordEditor, Required(true), DisplayName("Confirm Password")]
    public string ConfirmPassword { get; set; }
}