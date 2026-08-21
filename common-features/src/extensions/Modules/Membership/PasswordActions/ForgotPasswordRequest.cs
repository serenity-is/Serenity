namespace Serenity.Extensions;

/// <summary>
/// The request model for a forgot password service.
/// </summary>
[FormScript(LocalTextPrefix = "Forms.Membership.ForgotPassword.")]
public class ForgotPasswordRequest : ServiceRequest
{
    /// <summary>
    /// The email address of the account to reset the password for.
    /// </summary>
    [Required(true), EmailAddressEditor, DisplayName("Email Address"), Placeholder("email")]
    public string Email { get; set; }
}