namespace Serenity.Extensions;

/// <summary>
/// The response model for a reset password service.
/// </summary>
[ScriptInclude]
public class ResetPasswordResponse : ServiceResponse
{
    /// <summary>
    /// Whether the user should be redirected to the home page after resetting the password.
    /// </summary>
    public bool RedirectHome { get; set; }
}