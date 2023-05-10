using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Serenity.Web;

internal class AutoValidateAntiforgeryIgnoreBearerFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
{
    private const string AntiforgeryTokenInvalid = "Antiforgery token validation failed. {Message}";
    private const string NotMostEffectiveFilter = "Skipping the execution of current filter as its not the most effective filter implementing the policy {FilterPolicy}.";

    private readonly IAntiforgery antiforgery;
    private readonly ILogger logger;

    public AutoValidateAntiforgeryIgnoreBearerFilter(IAntiforgery antiforgery, ILoggerFactory loggerFactory)
    {
        if (antiforgery == null)
            throw new ArgumentNullException(nameof(antiforgery));

        this.antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
        logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
        {
            logger?.LogTrace(NotMostEffectiveFilter, typeof(IAntiforgeryPolicy));
            return;
        }

        if (ShouldValidate(context))
        {
            try
            {
                await antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException exception)
            {
                logger?.LogWarning(exception, AntiforgeryTokenInvalid, exception.Message);
                context.Result = new AntiforgeryValidationFailedResult();
            }
        }
    }

    protected virtual bool ShouldValidate(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var method = context.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method) ||
            HttpMethods.IsTrace(method) ||
            HttpMethods.IsOptions(method))
        {
            return false;
        }

        string authorization = context.HttpContext.Request.Headers[HeaderNames.Authorization];
        var cookie = context.HttpContext.Request.Headers[HeaderNames.Cookie];
        
        if (!string.IsNullOrEmpty(authorization) && 
            authorization.StartsWith("Bearer ", StringComparison.InvariantCulture) && 
            string.IsNullOrEmpty(cookie))
        {
            return false;
        }

        return true;
    }
}
