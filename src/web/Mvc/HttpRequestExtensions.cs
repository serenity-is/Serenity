using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Contains HTTP request related extensions.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the base URI for the current request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="pathBase">Whether to include the path base.</param>
    /// <returns>The base URI.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
    public static Uri GetBaseUri(this HttpRequest request, bool pathBase = true)
    {
        ArgumentNullException.ThrowIfNull(request);

        var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, 
            request.Host.Port ?? -1, pathBase ? request.PathBase : "");
            
        if (uriBuilder.Uri.IsDefaultPort)
            uriBuilder.Port = -1;

        return uriBuilder.Uri;
    }
}