using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// Requires a user to change the password, e.g. one that an administrator set, before using the application, when the
/// user definition says so (see <see cref="IHasPasswordChangeRequired"/>) and the user signed in with the password.
/// </summary>
/// <param name="userRetriever">Gives the user definitions.</param>
public class PasswordChangeRequirement(IUserRetrieveService userRetriever) : IAccountSetupRequirement
{
    /// <inheritdoc/>
    public virtual int Order => 10;

    /// <inheritdoc/>
    public virtual string SetupUrl => "~/Account/ChangePassword";

    /// <inheritdoc/>
    public virtual bool IsRequired(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);
        // a sign-in with an external provider has no password of the application to change
        return string.Equals(user.Identity?.AuthenticationType, "Password", StringComparison.OrdinalIgnoreCase) &&
            userRetriever.GetUserDefinition(user) is IHasPasswordChangeRequired { PasswordChangeRequired: true };
    }

    /// <inheritdoc/>
    public virtual string GetMessage(ITextLocalizer localizer) => AccountSetupTexts.PasswordChangeRequired.ToString(localizer);
}
