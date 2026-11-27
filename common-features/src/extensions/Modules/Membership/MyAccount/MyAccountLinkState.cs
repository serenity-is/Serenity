namespace Serenity.Extensions;

/// <summary>
/// The state of what a link of the my account page is for, which its badge shows, see <see cref="MyAccountLink.State"/>.
/// </summary>
public enum MyAccountLinkState
{
    /// <summary>No badge.</summary>
    None = 0,

    /// <summary>It is on, e.g. two-factor authentication is set up.</summary>
    On = 1,

    /// <summary>It is off.</summary>
    Off = 2,

    /// <summary>The user must do something, e.g. set up two-factor authentication that is required.</summary>
    Warning = 3
}
