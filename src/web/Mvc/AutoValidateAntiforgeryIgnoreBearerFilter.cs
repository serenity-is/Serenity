using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Serenity.Web;

internal class AutoValidateAntiforgeryIgnoreBearerFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
{
    private const string AntiforgeryTokenInvalid = "Antiforgery token validation failed. {Message}";
    private const string NotMostEffectiveFilter = "Skipping the execution of current filter as its not the most effective filter implementing the policy {FilterPolicy}.";

    private readonly IAntiforgery antiforgery;
    private readonly IOptions<AntiforgeryFilterOptions> options;
    private readonly ILogger logger;

    public AutoValidateAntiforgeryIgnoreBearerFilter(
        IAntiforgery antiforgery, 
        ILoggerFactory loggerFactory,
        IOptions<AntiforgeryFilterOptions> options)
    {
        ArgumentNullException.ThrowIfNull(antiforgery);

        this.antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
        this.options = options;
        logger = loggerFactory?.CreateLogger(GetType());
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
        {
            if (logger?.IsEnabled(LogLevel.Trace) == true)
                logger.LogTrace(NotMostEffectiveFilter, typeof(IAntiforgeryPolicy));
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
                if (logger?.IsEnabled(LogLevel.Warning) == true)
                    logger.LogWarning(exception, AntiforgeryTokenInvalid, exception.Message);
                context.Result = new AntiforgeryValidationFailedResult();
            }
        }
    }

    protected virtual bool ShouldValidate(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

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

        if (!string.IsNullOrEmpty(options?.Value?.SkipValidationHeaderName))
        {
            var headerValue = context.HttpContext.Request.Headers[options.Value.SkipValidationHeaderName];
            if (!string.IsNullOrEmpty(headerValue))
            {
                if (string.IsNullOrEmpty(options.Value.SkipValidationHeaderValue) ||
                    string.Equals(headerValue, options.Value.SkipValidationHeaderValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
