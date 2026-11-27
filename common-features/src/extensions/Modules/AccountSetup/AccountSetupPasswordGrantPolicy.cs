namespace Serenity.Extensions;

/// <summary>
/// Refuses a token with the password, e.g. the password grant of a token endpoint, to a user who must set something up
/// before using the application, e.g. change a password that an administrator set (see <see cref="IAccountSetupRequirement"/>),
/// as a token is used without the pages, so <see cref="AccountSetupFilter"/> can't keep the user to the page that does it.
/// The user signs in with the browser once and does it.
/// </summary>
/// <param name="requirements">The requirements.</param>
/// <param name="claimCreator">Creates the principal of a sign-in with the password, to ask the requirements.</param>
/// <param name="localizer">The localizer of the message.</param>
public class AccountSetupPasswordGrantPolicy(IEnumerable<IAccountSetupRequirement> requirements, IUserClaimCreator claimCreator,
    ITextLocalizer localizer) : IPasswordGrantPolicy
{
    private readonly IAccountSetupRequirement[] requirements = [.. requirements.OrderBy(x => x.Order)];

    /// <inheritdoc/>
    public virtual string? GetDenial(IUserDefinition user)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (requirements.Length == 0)
            return null;

        // as if the user signed in with the password on the sign-in page
        var principal = claimCreator.CreatePrincipal(user.Username, authType: "Password");
        return requirements.FirstOrDefault(x => x.IsRequired(principal))?.GetMessage(localizer);
    }
}
