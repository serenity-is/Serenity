namespace Serenity.Extensions;

[ScriptInclude]
public class PasswordStrengthRules
{
    public int MinPasswordLength { get; set; }
    public bool RequireDigit { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireUppercase { get; set; }
}