namespace Serenity.Extensions;

/// <summary>
/// A button of an email, see the <c>_EmailButton</c> view, e.g. the link that resets the password.
/// </summary>
public class EmailButtonModel
{
    /// <summary>The text of the button.</summary>
    public string? Text { get; set; }

    /// <summary>The address it opens.</summary>
    public string? Url { get; set; }
}
