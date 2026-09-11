namespace Serenity.Web;

public class RecaptchaValidationTests
{
    [Fact]
    public void Validate_Throws_When_Token_Is_Empty()
    {
        var exception = Assert.Throws<ValidationError>(() =>
            RecaptchaValidation.Validate("secret", "", NullTextLocalizer.Instance));

        Assert.Equal("Recaptcha", exception.ErrorCode);
    }

    [Fact]
    public void Validate_Throws_When_Token_Is_Null()
    {
        var exception = Assert.Throws<ValidationError>(() =>
            RecaptchaValidation.Validate("secret", null!, NullTextLocalizer.Instance));

        Assert.Equal("Recaptcha", exception.ErrorCode);
    }
}
