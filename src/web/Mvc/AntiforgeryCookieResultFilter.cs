using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services;

/// <summary>
/// Append the anti forgery token as CSRF-TOKEN cookie to the response, 
/// so that AJAX calls can read it client side and send as a header.
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="antiforgery">Antiforgery service</param>
public class AntiforgeryCookieResultFilterAttribute(IAntiforgery antiforgery) : ResultFilterAttribute
{
    private readonly IAntiforgery antiforgery = antiforgery;

    /// <inheritdoc/>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ViewResult)
        {
            var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken, 
                new CookieOptions() { HttpOnly = false, SameSite = SameSiteMode.Lax });
        }
    }
}