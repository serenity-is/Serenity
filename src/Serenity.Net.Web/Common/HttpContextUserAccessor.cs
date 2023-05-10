using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IUserAccessor"/> for HTTP context
/// </summary>
public class HttpContextUserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor = null)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public ClaimsPrincipal User => httpContextAccessor?.HttpContext?.User;
}

