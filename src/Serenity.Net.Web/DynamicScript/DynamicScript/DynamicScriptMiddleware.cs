using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Serenity.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Serenity.Web.Middleware
{
    public class DynamicScriptMiddleware
    {
        private readonly RequestDelegate next;
        const string dynJSPath = "/DynJS.axd/";

        public DynamicScriptMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Value.StartsWith(dynJSPath, StringComparison.OrdinalIgnoreCase))
                return next.Invoke(context);

            var scriptKey = context.Request.Path.Value;
            scriptKey = scriptKey[dynJSPath.Length..];

            var contentType = "text/javascript";
            if (scriptKey.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                scriptKey = scriptKey[0..^3];
            else if (scriptKey.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "text/css";
                scriptKey = scriptKey[0..^4];
            }

            return ReturnScript(context, scriptKey, contentType);
        }

        public async static Task ReturnScript(HttpContext context, string scriptKey, string contentType)
        {
            IScriptContent scriptContent;
            try
            {
                scriptContent = context.RequestServices.GetRequiredService<IDynamicScriptManager>()
                    .ReadScriptContent(scriptKey);
            }
            catch (ValidationError ve)
            {
                if (ve.ErrorCode == "AccessDenied")
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                throw;
            }

            if (scriptContent == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("File not found!");
                return;
            }

            var mediaType = new MediaTypeHeaderValue(contentType)
            {
                Encoding = System.Text.Encoding.UTF8
            };
            context.Response.ContentType = mediaType.ToString();

            var responseHeaders = context.Response.GetTypedHeaders();
            var cacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromDays(365)
            };

            // allow CDNs to cache anonymous resources
            if (!string.IsNullOrEmpty(context.Request.Query["v"]) &&
                context.User?.IsLoggedIn() != true)
                cacheControl.Public = true;
            else                
                cacheControl.Private = true;

            cacheControl.MustRevalidate = false;
            responseHeaders.CacheControl = cacheControl;

            var supportsGzip = scriptContent.CanCompress && 
                context.Request.Headers["Accept-Encoding"].Any(x => x.Contains("gzip", StringComparison.Ordinal));

            byte[] contentBytes;
            if (supportsGzip)
            {
                context.Response.Headers["Content-Encoding"] = "gzip";
                contentBytes = scriptContent.CompressedContent;
            }
            else
                contentBytes = scriptContent.Content;

            await WriteWithIfModifiedSinceControl(context, contentBytes,
                scriptContent.Time);
        }

        public async static Task WriteWithIfModifiedSinceControl(HttpContext context, byte[] bytes, DateTime lastWriteTime)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
            if (ifModifiedSince != null && ifModifiedSince.Length > 0)
            {
                if (DateTime.TryParseExact(ifModifiedSince, "R", Invariants.DateTimeFormat, DateTimeStyles.None,
                    out DateTime date))
                {
                    if (date.Year == lastWriteTime.Year &&
                        date.Month == lastWriteTime.Month &&
                        date.Day == lastWriteTime.Day &&
                        date.Hour == lastWriteTime.Hour &&
                        date.Minute == lastWriteTime.Minute &&
                        date.Second == lastWriteTime.Second)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                        return;
                    }
                }
            }

            var utcNow = DateTime.UtcNow;
            if (lastWriteTime >= utcNow)
                lastWriteTime = utcNow;

            context.Response.GetTypedHeaders().LastModified = lastWriteTime;
            await context.Response.Body.WriteAsync(bytes.AsMemory(0, bytes.Length));
        }
    }
}
