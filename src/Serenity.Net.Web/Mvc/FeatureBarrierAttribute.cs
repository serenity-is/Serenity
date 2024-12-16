using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// An attribute that can be placed on MVC controllers, controller actions, or Razor pages to require all or any of a set of features to be enabled.
/// Note that if no <see cref="IFeatureToggles"/> service is registered, the feature barrier will always pass.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class FeatureBarrierAttribute : RequiresFeatureAttribute, IActionConstraint, IPageFilter
{
    /// <summary>
    /// Creates an attribute that can be used to barrier actions or pages. The barrier can be configured to require all or any of the provided feature(s) to pass.
    /// </summary>
    /// <param name="features">The names of the features that the attribute will represent.</param>
    public FeatureBarrierAttribute(params string[] features) : base(features)
    {
    }

    /// <summary>
    /// Creates an attribute that can be used to barrier actions or pages. The barrier can be configured to require all or any of the provided feature(s) to pass.
    /// </summary>
    /// <param name="features">A set of enums representing the features that the attribute will represent.</param>
    public FeatureBarrierAttribute(params object[] features) : base(features)
    {
    }

    /// <inheritdoc/>
    public int Order => 0;

    /// <inheritdoc/>
    public bool Accept(ActionConstraintContext context)
    {
        var featureToggles = context.RouteContext.HttpContext
            .RequestServices.GetService<IFeatureToggles>();
        if (featureToggles != null && 
            !featureToggles.IsEnabled(Features, RequireAny))
            return false;

        return true;
    }

    /// <inheritdoc/>
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
    }

    /// <inheritdoc/>
    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var featureToggles = context.HttpContext.RequestServices.GetService<IFeatureToggles>();
        if (featureToggles is not null &&
            !featureToggles.IsEnabled(Features, RequireAny))
        {
            context.Result = new NotFoundResult();
        }
    }

    /// <inheritdoc/>
    public void OnPageHandlerSelected(PageHandlerSelectedContext context)
    {
    }
}
