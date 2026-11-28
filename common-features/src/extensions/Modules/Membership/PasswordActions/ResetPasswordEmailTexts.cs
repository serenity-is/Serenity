namespace Serenity.Extensions;

/// <summary>
/// The texts of the email with the link that resets the password, see the <c>ResetPasswordEmail</c> view. Its subject is
/// <see cref="ResetPasswordFormTexts.EmailSubject"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Emails.ResetPassword.")]
public static class ResetPasswordEmailTexts
{
    /// <summary>The button that opens the page that sets the new password.</summary>
    public static readonly LocalText Button = "Set a new password";

    /// <summary>How long the link works, in hours.</summary>
    public static readonly LocalText Expires = "The link works for {0} hours.";

    /// <summary>The greeting, with the name of the user.</summary>
    public static readonly LocalText Greeting = "Hello {0},";

    /// <summary>What to do if the user didn't ask for it.</summary>
    public static readonly LocalText Ignore = "If you didn't ask for this, you can ignore this email. Your password stays the same.";

    /// <summary>Why the email was sent, with the title of the site.</summary>
    public static readonly LocalText Intro = "We received a request to change the password of your account on {0}. Use the button below to set a new one.";

    /// <summary>Why the email was sent, when the site has no title.</summary>
    public static readonly LocalText IntroNoSite = "We received a request to change the password of your account. Use the button below to set a new one.";
}
