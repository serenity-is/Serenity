using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Serenity.Services;

/// <summary>
/// Append the anti forgery token as CSRF-TOKEN cookie to the response, 
/// so that AJAX calls can read it client side and send as a header.
/// </summary>
public class AntiforgeryCookieResultFilterAttribute : ResultFilterAttribute
{
    private readonly IAntiforgery antiforgery;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="antiforgery">Antiforgery service</param>
    public AntiforgeryCookieResultFilterAttribute(IAntiforgery antiforgery)
    {
        this.antiforgery = antiforgery;
    }

    /// <inheritdoc/>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ViewResult)
        {
            var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken, 
                new CookieOptions() { HttpOnly = false });
        }
    }
}