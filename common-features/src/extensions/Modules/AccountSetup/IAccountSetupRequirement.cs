using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Something that a signed in user must do before using the application, e.g. change a password that an administrator
/// set, or set up two-factor authentication. <see cref="AccountSetupFilter"/> keeps the user to the page that does it,
/// until it is done. Register one with <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;IAccountSetupRequirement, MyRequirement&gt;())</c>.
/// </summary>
public interface IAccountSetupRequirement
{
    /// <summary>
    /// The order of the requirement among the others, the first that a user must do is asked first, e.g. a password
    /// change (10) before the setup of two-factor authentication (20).
    /// </summary>
    int Order { get; }

    /// <summary>
    /// The page where the user does it, e.g. "~/Account/ChangePassword".
    /// </summary>
    string SetupUrl { get; }

    /// <summary>
    /// Whether the user must do it now. It is called on each request of a signed in user, so it should be fast, e.g. use the
    /// cached user definition.
    /// </summary>
    /// <param name="user">The signed in user.</param>
    bool IsRequired(ClaimsPrincipal user);

    /// <summary>
    /// The message of a service that is refused until it is done.
    /// </summary>
    /// <param name="localizer">The localizer.</param>
    string GetMessage(ITextLocalizer localizer);
}
