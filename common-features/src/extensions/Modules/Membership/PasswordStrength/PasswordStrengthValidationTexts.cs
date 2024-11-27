namespace Serenity.Extensions;

[NestedLocalTexts(Prefix = "Validation.")]
public static class PasswordStrengthValidationTexts
{
    public static readonly LocalText MinRequiredPasswordLength = "Entered password doesn't have enough characters (min {0})!";
    public static readonly LocalText PasswordStrengthRequireDigit = "Password must contain a digit (0-9).";
    public static readonly LocalText PasswordStrengthRequireLowercase = "Password must contain a lowercase (a-z) letter.";
    public static readonly LocalText PasswordStrengthRequireUppercase = "Password must contain an uppercase (A-Z) letter.";
    public static readonly LocalText PasswordStrengthRequireNonAlphanumeric = "Password must contain a non-alphanumeric character.";
}