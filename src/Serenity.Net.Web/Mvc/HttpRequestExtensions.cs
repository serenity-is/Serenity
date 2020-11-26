using Microsoft.AspNetCore.Http;
using System;

namespace Serenity.Web
{
    public static class HttpRequestExtensions
    {
        public static Uri GetBaseUri(this HttpRequest request)
        {
            var hostComponents = request.Host.ToUriComponent().Split(':');

            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = hostComponents[0]
            };

            if (hostComponents.Length == 2)
                builder.Port = Convert.ToInt32(hostComponents[1]);

            return builder.Uri;
        }
    }
}