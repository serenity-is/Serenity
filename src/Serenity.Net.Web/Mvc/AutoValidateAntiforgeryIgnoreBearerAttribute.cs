using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// An attribute that causes validation of antiforgery tokens for all unsafe HTTP methods. An antiforgery
/// token is required for HTTP methods other than GET, HEAD, OPTIONS, and TRACE, and when
/// there is no cookie header and authorization is Bearer, e.g. JWT etc.
/// </summary>
/// <remarks>
/// <see cref="AutoValidateAntiforgeryIgnoreBearerAttribute"/> can be applied as a global filter to trigger
/// validation of antiforgery tokens by default for an application. Use
/// <see cref="Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute"/> to suppress validation of the antiforgery token for
/// a controller or action.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AutoValidateAntiforgeryIgnoreBearerAttribute : Attribute, IFilterFactory, IOrderedFilter
{
    /// <inheritdoc/>
    public int Order { get; set; } = 1000;

    /// <inheritdoc />
    public bool IsReusable => true;

    /// <inheritdoc />
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return ActivatorUtilities.CreateInstance<AutoValidateAntiforgeryIgnoreBearerFilter>(serviceProvider);
    }
}
