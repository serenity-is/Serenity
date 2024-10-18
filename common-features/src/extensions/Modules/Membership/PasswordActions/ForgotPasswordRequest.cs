namespace Serenity.Extensions;

[FormScript(LocalTextPrefix = "Forms.Membership.ForgotPassword.")]
public class ForgotPasswordRequest : ServiceRequest
{
    [Required(true), EmailAddressEditor, DisplayName("Email Address"), Placeholder("email")]
    public string Email { get; set; }
}