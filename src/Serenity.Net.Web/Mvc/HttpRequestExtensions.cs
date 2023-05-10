using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// Contains HTTP request related extensions
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the base uri for the current request
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <param name="pathBase">Include path base</param>
    /// <exception cref="ArgumentNullException">Request is null</exception>
    public static Uri GetBaseUri(this HttpRequest request, bool pathBase = true)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, 
            request.Host.Port ?? -1, pathBase ? request.PathBase : "");
            
        if (uriBuilder.Uri.IsDefaultPort)
            uriBuilder.Port = -1;

        return uriBuilder.Uri;
    }
}