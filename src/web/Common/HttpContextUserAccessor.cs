using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IUserAccessor"/> that reads the
/// current user from the HTTP context.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HttpContextUserAccessor"/> class.
/// </remarks>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
public class HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor = null) : IUserAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public ClaimsPrincipal User => httpContextAccessor?.HttpContext?.User;
}

