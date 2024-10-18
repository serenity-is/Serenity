using Microsoft.AspNetCore.Mvc;
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
        catch (Exception)
        {
            if (!string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
                throw;

            var userRetrieveService = context.HttpContext.RequestServices.GetRequiredService<IUserRetrieveService>();

            var path = context.HttpContext.Request.Path.Value;

            if (string.IsNullOrEmpty(path))
                path = "/";
                
            if (context.HttpContext.User.GetUserDefinition(userRetrieveService) is IHasPassword { HasPassword: false })
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