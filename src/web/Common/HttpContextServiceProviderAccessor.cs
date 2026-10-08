using Microsoft.AspNetCore.Http;

namespace Serenity;

/// <summary>
/// An <see cref="IServiceProviderAccessor"/> that uses the current HTTP request's scoped service
/// provider when available, falling back to the root provider otherwise.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="serviceProvider">Root service provider used when there is no active HTTP request.</param>
/// <param name="httpContextAccessor">HTTP context accessor.</param>
/// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <c>null</c>.</exception>
public class HttpContextServiceProviderAccessor(IServiceProvider serviceProvider, IHttpContextAccessor? httpContextAccessor = null) : IServiceProviderAccessor
{
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider => httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider;
}
