namespace Serenity.Extensions;

/// <summary>
/// Settings for membership.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class MembershipSettings
{
    /// <summary>
    /// Default section key for membership settings.
    /// </summary>
    public const string SectionKey = "Membership";

    /// <summary>
    /// The minimum required password length.
    /// </summary>
    public int MinPasswordLength { get; set; } = 6;
    /// <summary>
    /// Whether passwords must contain a digit.
    /// </summary>
    public bool RequireDigit { get; set; } = true;
    /// <summary>
    /// Whether passwords must contain a lowercase letter.
    /// </summary>
    public bool RequireLowercase { get; set; } = true;
    /// <summary>
    /// Whether passwords must contain a non-alphanumeric character.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; } = true;
    /// <summary>
    /// Whether passwords must contain an uppercase letter.
    /// </summary>
    public bool RequireUppercase { get; set; } = true;
    /// <summary>
    /// The size of the salt used when hashing passwords.
    /// </summary>
    public int SaltSize { get; set; } = 5;
}
