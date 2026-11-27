namespace Serenity.Extensions;

/// <summary>
/// A link of the my account page, to a page where the user manages something of the account, e.g. the password or the
/// two-factor authentication, which an <see cref="IMyAccountLinkProvider"/> gives.
/// </summary>
public class MyAccountLink
{
    /// <summary>A key, e.g. "Password", to find it, e.g. in a test or the styles.</summary>
    public string? Key { get; set; }

    /// <summary>The order among the others, the lowest first.</summary>
    public int Order { get; set; }

    /// <summary>The icon class, e.g. "fa fa-key".</summary>
    public string? Icon { get; set; }

    /// <summary>The title, localized.</summary>
    public string? Title { get; set; }

    /// <summary>What it is for, localized.</summary>
    public string? Description { get; set; }

    /// <summary>The address of the page, e.g. "~/Account/ChangePassword".</summary>
    public string? Url { get; set; }

    /// <summary>The state, which the badge shows, see <see cref="StateText"/>.</summary>
    public MyAccountLinkState State { get; set; }

    /// <summary>The text of the badge, localized, e.g. "On".</summary>
    public string? StateText { get; set; }
}
