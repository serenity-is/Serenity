namespace Serenity.Extensions;

/// <summary>
/// Local text keys for the forgot password form.
/// </summary>
[NestedLocalTexts(Prefix = "Forms.Membership.ForgotPassword.")]
public static class ForgotPasswordFormTexts
{
    public static readonly LocalText FormInfo = "Please enter the email you used to signup.";
    public static readonly LocalText FormTitle = "Forgot My Password";
    public static readonly LocalText SubmitButton = "Reset My Password";
    public static readonly LocalText SuccessMessage = "If this user exists, we have sent you an email with password reset instructions.";
}