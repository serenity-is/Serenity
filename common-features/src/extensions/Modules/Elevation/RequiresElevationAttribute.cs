using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Used for critical methods that needs additional authentication. Like account link/unlinking.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresElevationAttribute : Attribute, IResourceFilter
{
    /// <inheritdoc />
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var elevationHandler = context.HttpContext.RequestServices.GetRequiredService<IElevationHandler>();

        var method = context.HttpContext.Request.Method;
        
        try
        {
            elevationHandler.ValidateElevationToken();
        }
        catch (Exception ex)
        {
            if (!string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
                throw;

            // an impersonator can't confirm the password of the impersonated user, so the page is not available
            if (ex is ValidationError { ErrorCode: Serenity.Extensions.ImpersonationExtensions.RestrictedCode })
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ContentResult
                {
                    StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden,
                    ContentType = "text/plain; charset=utf-8",
                    Content = ex.Message
                };
                return;
            }

            var userRetrieveService = context.HttpContext.RequestServices.GetRequiredService<IUserRetrieveService>();

            var path = context.HttpContext.Request.Path.Value;

            if (string.IsNullOrEmpty(path))
                path = "/";
                
            if (userRetrieveService.GetUserDefinition(context.HttpContext.User) is IHasPassword { HasPassword: false })
                context.Result = new LocalRedirectResult("~/Account/SetPassword?reason=elevate");
            else
                context.Result = new LocalRedirectResult("~/Account/Elevate?returnUrl=" + Uri.EscapeDataString(path));
        }
        
    }

    /// <inheritdoc />
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}