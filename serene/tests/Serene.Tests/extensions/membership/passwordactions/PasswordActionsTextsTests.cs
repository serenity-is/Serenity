namespace Serenity.Extensions;

public class PasswordActionsTextsTests
{
    [Fact]
    public void ChangePasswordFormTexts_Are_Accessible()
    {
        Assert.NotNull(ChangePasswordFormTexts.FormTitle);
        Assert.NotNull(ChangePasswordFormTexts.SubmitButton);
        Assert.NotNull(ChangePasswordFormTexts.Success);
    }

    [Fact]
    public void ChangePasswordValidationTexts_Are_Accessible()
    {
        Assert.NotNull(ChangePasswordValidationTexts.InvalidResetToken);
        Assert.NotNull(ChangePasswordValidationTexts.PasswordConfirmMismatch);
    }

    [Fact]
    public void ForgotPasswordFormTexts_Are_Accessible()
    {
        Assert.NotNull(ForgotPasswordFormTexts.FormInfo);
        Assert.NotNull(ForgotPasswordFormTexts.FormTitle);
        Assert.NotNull(ForgotPasswordFormTexts.SubmitButton);
        Assert.NotNull(ForgotPasswordFormTexts.SuccessMessage);
    }

    [Fact]
    public void ResetPasswordFormTexts_Are_Accessible()
    {
        Assert.NotNull(ResetPasswordFormTexts.EmailSubject);
        Assert.NotNull(ResetPasswordFormTexts.FormTitle);
        Assert.NotNull(ResetPasswordFormTexts.SubmitButton);
        Assert.NotNull(ResetPasswordFormTexts.Success);
    }

    [Fact]
    public void SetPasswordFormTexts_Are_Accessible()
    {
        Assert.NotNull(SetPasswordFormTexts.ElevatedActionsMessage);
        Assert.NotNull(SetPasswordFormTexts.EmailSentMessage);
        Assert.NotNull(SetPasswordFormTexts.EmailToSetPasswordMessage);
        Assert.NotNull(SetPasswordFormTexts.PageTitle);
        Assert.NotNull(SetPasswordFormTexts.SendEmailButton);
    }
}
