namespace Serenity.Extensions;

[FormScript(LocalTextPrefix = "Forms.Membership.ResetPassword.")]
public class ResetPasswordRequest : ServiceRequest
{
    [Ignore]
    public string Token { get; set; }
    [PasswordEditor, Required(true), DisplayName("New Password"), Placeholder("password")]
    public string NewPassword { get; set; }
    [PasswordEditor, Required(true), DisplayName("Confirm Password"), Placeholder("confirm password")]
    public string ConfirmPassword { get; set; }
}