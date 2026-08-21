
namespace Serenity.Extensions;

/// <summary>
/// The model for the reset password email.
/// </summary>
public class ResetPasswordEmailModel
{
    /// <summary>
    /// The display name of the user.
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// The reset password link to include in the email.
    /// </summary>
    public string ResetLink { get; set; }
}