namespace Serenity.Extensions;

/// <summary>
/// The texts of the layout of the emails, see <c>~/Areas/Serenity.Extensions/Shared/_EmailLayout.cshtml</c>.
/// </summary>
[NestedLocalTexts(Prefix = "Emails.Layout.")]
public static class EmailLayoutTexts
{
    /// <summary>The line under the content, that the email was sent automatically.</summary>
    public static readonly LocalText Automatic = "This email was sent automatically.";

    /// <summary>The signature, with the title of the site.</summary>
    public static readonly LocalText Signature = "The {0} team";
}
