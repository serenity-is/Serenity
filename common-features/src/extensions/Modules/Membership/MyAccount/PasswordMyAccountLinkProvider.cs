using System.Security.Claims;

namespace Serenity.Extensions;

/// <summary>
/// The link of the my account page to the page that changes the password, or sets one for a user who has none, e.g. one
/// who signs in with an external provider, see <see cref="IHasPassword"/>.
/// </summary>
/// <param name="userRetriever">Gives the user definitions.</param>
/// <param name="localizer">The localizer of the texts.</param>
public class PasswordMyAccountLinkProvider(IUserRetrieveService userRetriever, ITextLocalizer localizer) : IMyAccountLinkProvider
{
    /// <inheritdoc/>
    public virtual IEnumerable<MyAccountLink> GetLinks(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var definition = userRetriever.GetUserDefinition(user);
        var hasPassword = definition is not IHasPassword { HasPassword: false };
        var changeRequired = definition is IHasPasswordChangeRequired { PasswordChangeRequired: true };
        yield return new MyAccountLink
        {
            Key = "Password",
            Order = 10,
            Icon = "fa fa-key",
            Title = MyAccountTexts.PasswordTitle.ToString(localizer),
            Description = (hasPassword ? MyAccountTexts.ChangePasswordDescription : MyAccountTexts.SetPasswordDescription)
                .ToString(localizer),
            // the page sets a password for a user who has none
            Url = "~/Account/ChangePassword",
            State = changeRequired ? MyAccountLinkState.Warning : MyAccountLinkState.None,
            StateText = changeRequired ? MyAccountTexts.ChangePasswordRequired.ToString(localizer) : null
        };
    }
}
