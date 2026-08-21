namespace Serenity.Extensions;

/// <summary>
/// The password strength rules.
/// </summary>
[ScriptInclude]
public class PasswordStrengthRules
{
    /// <summary>
    /// The minimum required password length.
    /// </summary>
    public int MinPasswordLength { get; set; }
    /// <summary>
    /// Whether the password must contain a digit.
    /// </summary>
    public bool RequireDigit { get; set; }
    /// <summary>
    /// Whether the password must contain a lowercase letter.
    /// </summary>
    public bool RequireLowercase { get; set; }
    /// <summary>
    /// Whether the password must contain a non-alphanumeric character.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; }
    /// <summary>
    /// Whether the password must contain an uppercase letter.
    /// </summary>
    public bool RequireUppercase { get; set; }
}