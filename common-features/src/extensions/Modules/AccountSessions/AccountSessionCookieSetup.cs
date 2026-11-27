using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

/// <summary>
/// Calls the <see cref="AccountSessionCookieValidator"/> from the events of the default cookie scheme, after the handlers
/// that the application set, if any, so the sessions work without a change in the code that signs the users in. An
/// application that gives its events as a type (<c>EventsType</c>) calls the validator from them instead.
/// </summary>
public class AccountSessionCookieSetup : IPostConfigureOptions<CookieAuthenticationOptions>
{
    /// <inheritdoc/>
    public void PostConfigure(string? name, CookieAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (name != CookieAuthenticationDefaults.AuthenticationScheme)
            return;

        var events = options.Events ??= new CookieAuthenticationEvents();
        var signingIn = events.OnSigningIn;
        var validate = events.OnValidatePrincipal;
        var signingOut = events.OnSigningOut;

        events.OnSigningIn = async context =>
        {
            await signingIn(context).ConfigureAwait(false);
            context.HttpContext.RequestServices.GetService<AccountSessionCookieValidator>()?.SigningIn(context);
        };

        events.OnValidatePrincipal = async context =>
        {
            await validate(context).ConfigureAwait(false);
            if (context.Principal != null && context.HttpContext.RequestServices.GetService<AccountSessionCookieValidator>() is { } validator)
                await validator.ValidatePrincipalAsync(context).ConfigureAwait(false);
        };

        events.OnSigningOut = async context =>
        {
            await signingOut(context).ConfigureAwait(false);
            context.HttpContext.RequestServices.GetService<AccountSessionCookieValidator>()?.SigningOut(context);
        };
    }
}
