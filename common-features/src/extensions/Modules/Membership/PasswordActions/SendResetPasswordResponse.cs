namespace Serenity.Extensions;

/// <summary>
/// The response model for a send reset password service.
/// </summary>
[ScriptInclude]
public class SendResetPasswordResponse : ServiceResponse
{
    /// <summary>
    /// The demo reset link, only returned in public demo mode.
    /// </summary>
    public string DemoLink { get; set; }    
}