using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Extensions;

/// <summary>
/// Keeps a signed in user who must still set up the account, e.g. change a password that an administrator set, from the
/// application until it is done, by the first <see cref="IAccountSetupRequirement"/> that the user must do: a page
/// redirects to the page that does it, and a service fails with the <see cref="SetupRequiredCode"/> error code, whose
/// arguments are the path of that page, which a page handles by going there. The actions with
/// <see cref="AllowDuringAccountSetupAttribute"/> are allowed. It is checked on each request, so a requirement takes
/// effect at once, and ends when it is done.
/// </summary>
/// <param name="requirements">The requirements.</param>
/// <param name="localizer">The localizer of the messages.</param>
public class AccountSetupFilter(IEnumerable<IAccountSetupRequirement> requirements, ITextLocalizer localizer) : IAuthorizationFilter
{
    /// <summary>The error code of a service that is refused until the account is set up.</summary>
    public const string SetupRequiredCode = "AccountSetupRequired";

    private readonly IAccountSetupRequirement[] requirements = requirements?.OrderBy(x => x.Order).ToArray() ?? [];

    /// <inheritdoc/>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var user = context.HttpContext.User;
        if (requirements.Length == 0 ||
            user?.Identity?.IsAuthenticated != true ||
            context.ActionDescriptor.EndpointMetadata.OfType<AllowDuringAccountSetupAttribute>().Any())
            return;

        var requirement = requirements.FirstOrDefault(x => x.IsRequired(user));
        if (requirement == null)
            return;

        var request = context.HttpContext.Request;
        var setupPath = request.PathBase.Add(requirement.SetupUrl.StartsWith("~/", StringComparison.Ordinal) ?
            requirement.SetupUrl[1..] : requirement.SetupUrl).Value!;
        var isService = request.Path.StartsWithSegments("/Services", StringComparison.OrdinalIgnoreCase) ||
            !HttpMethods.IsGet(request.Method);
        if (isService)
        {
            context.Result = new JsonResult(new ServiceResponse
            {
                Error = new ServiceError
                {
                    Code = SetupRequiredCode,
                    Arguments = setupPath,
                    Message = requirement.GetMessage(localizer)
                }
            })
            { StatusCode = StatusCodes.Status400BadRequest };
        }
        else
            context.Result = new LocalRedirectResult(requirement.SetupUrl);
    }
}
