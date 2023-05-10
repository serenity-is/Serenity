using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IHttpContextItemsAccessor"/>
/// using <see cref="IHttpContextAccessor"/> and its Items property.
/// </summary>
public class HttpContextItemsAccessor : IHttpContextItemsAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public HttpContextItemsAccessor(IHttpContextAccessor httpContextAccessor = null)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public IDictionary<object, object> Items => httpContextAccessor?.HttpContext?.Items;
}

