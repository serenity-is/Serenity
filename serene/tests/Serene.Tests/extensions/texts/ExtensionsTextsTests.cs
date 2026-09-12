namespace Serenity.Extensions;

public class ExtensionsTextsTests
{
    [Fact]
    public void AllTextKeys_Are_Accessible()
    {
        Assert.NotNull(ExtensionsTexts.Forms.Membership.ChangePassword.FormTitle);
        Assert.NotNull(ExtensionsTexts.Site.Translation.EntityPlural);
        Assert.NotNull(ExtensionsTexts.Validation.InvalidResetToken);
        Assert.NotNull(ExtensionsTexts.Validation.PasswordConfirmMismatch);
    }
}
