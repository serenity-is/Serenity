namespace Serenity.Extensions;

public class PasswordStrengthValidatorTests
{
    private static PasswordStrengthValidator Create(MembershipSettings? settings = null)
    {
        return new PasswordStrengthValidator(
            Options.Create(settings ?? new MembershipSettings()),
            NullTextLocalizer.Instance);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PasswordStrengthValidator(null!, NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            new PasswordStrengthValidator(Options.Create(new MembershipSettings()), null!));
    }

    [Fact]
    public void Validate_Accepts_Strong_Password()
    {
        Create().Validate("Abcd1234!");
    }

    [Fact]
    public void Validate_Treats_Null_As_Empty_And_Throws_For_Short_Password()
    {
        var error = Assert.Throws<ValidationError>(() => Create().Validate(null!));
        Assert.Equal(nameof(MembershipSettings.MinPasswordLength), error.ErrorCode);
    }

    [Fact]
    public void Validate_Throws_When_Uppercase_Missing()
    {
        var error = Assert.Throws<ValidationError>(() => Create().Validate("abcd1234!"));
        Assert.Equal(nameof(MembershipSettings.RequireUppercase), error.ErrorCode);
    }

    [Fact]
    public void Validate_Throws_When_Lowercase_Missing()
    {
        var error = Assert.Throws<ValidationError>(() => Create().Validate("ABCD1234!"));
        Assert.Equal(nameof(MembershipSettings.RequireLowercase), error.ErrorCode);
    }

    [Fact]
    public void Validate_Throws_When_Digit_Missing()
    {
        var error = Assert.Throws<ValidationError>(() => Create().Validate("Abcdefg!"));
        Assert.Equal(nameof(MembershipSettings.RequireDigit), error.ErrorCode);
    }

    [Fact]
    public void Validate_Throws_When_NonAlphanumeric_Missing()
    {
        var error = Assert.Throws<ValidationError>(() => Create().Validate("Abcd12345"));
        Assert.Equal(nameof(MembershipSettings.RequireNonAlphanumeric), error.ErrorCode);
    }

    [Fact]
    public void Validate_Honors_Disabled_Rules()
    {
        var settings = new MembershipSettings
        {
            MinPasswordLength = 1,
            RequireUppercase = false,
            RequireLowercase = false,
            RequireDigit = false,
            RequireNonAlphanumeric = false
        };

        Create(settings).Validate("a");
    }
}
