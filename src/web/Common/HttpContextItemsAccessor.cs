using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IHttpContextItemsAccessor"/> that
/// reads the <see cref="IHttpContextAccessor.HttpContext"/> items.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HttpContextItemsAccessor"/> class.
/// </remarks>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
public class HttpContextItemsAccessor(IHttpContextAccessor httpContextAccessor = null) : IHttpContextItemsAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public IDictionary<object, object> Items => httpContextAccessor?.HttpContext?.Items;
}

