using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IUserAccessor"/> for HTTP context
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="httpContextAccessor">HTTP context accessor</param>
public class HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor = null) : IUserAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public ClaimsPrincipal User => httpContextAccessor?.HttpContext?.User;
}

