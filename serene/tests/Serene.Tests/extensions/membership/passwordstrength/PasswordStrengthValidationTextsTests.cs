namespace Serenity.Extensions;

public class PasswordStrengthValidationTextsTests
{
    [Fact]
    public void AllTextKeys_Are_Accessible()
    {
        Assert.NotNull(PasswordStrengthValidationTexts.MinRequiredPasswordLength);
        Assert.NotNull(PasswordStrengthValidationTexts.PasswordStrengthRequireDigit);
        Assert.NotNull(PasswordStrengthValidationTexts.PasswordStrengthRequireLowercase);
        Assert.NotNull(PasswordStrengthValidationTexts.PasswordStrengthRequireUppercase);
        Assert.NotNull(PasswordStrengthValidationTexts.PasswordStrengthRequireNonAlphanumeric);
    }
}
