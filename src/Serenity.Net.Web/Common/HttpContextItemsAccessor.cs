using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IHttpContextItemsAccessor"/>
/// using <see cref="IHttpContextAccessor"/> and its Items property.
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="httpContextAccessor">HTTP context accessor</param>
public class HttpContextItemsAccessor(IHttpContextAccessor httpContextAccessor = null) : IHttpContextItemsAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public IDictionary<object, object> Items => httpContextAccessor?.HttpContext?.Items;
}

