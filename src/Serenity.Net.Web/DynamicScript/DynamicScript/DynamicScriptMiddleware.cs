using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;

namespace Serenity.Web.Middleware;

/// <summary>
/// Dynamic script middleware that handles "/DynJS.axd/" and "/DynamicData/" paths.
/// </summary>
public class DynamicScriptMiddleware
{
    private readonly RequestDelegate next;
    const string dynJSPath = "/DynJS.axd/";
    const string dynamicDataPath = "/DynamicData/";

    /// <summary>
    /// Creates a new instance of the middleware
    /// </summary>
    /// <param name="next">Next request delegate</param>
    public DynamicScriptMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    /// <summary>
    /// Invokes the middleware in the context
    /// </summary>
    /// <param name="context">HTTP context</param>
    public Task Invoke(HttpContext context)
    {
        bool dynJS = context.Request.Path.Value.StartsWith(dynJSPath, StringComparison.OrdinalIgnoreCase);
        bool dynamicData = !dynJS && context.Request.Path.Value.StartsWith(dynamicDataPath, StringComparison.OrdinalIgnoreCase);

        if (!dynJS && !dynamicData)
            return next.Invoke(context);

        var scriptKey = context.Request.Path.Value;
        scriptKey = scriptKey[(dynJS ? dynJSPath.Length : dynamicDataPath.Length)..];

        string contentType;
        if (dynJS)
        {
            contentType = "text/javascript";
            if (scriptKey.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                scriptKey = scriptKey[0..^3];
            else if (scriptKey.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "text/css";
                scriptKey = scriptKey[0..^4];
            }
        }
        else
            contentType = "application/json";

        return ReturnScript(context, scriptKey, contentType, json: dynamicData);
    }

    /// <summary>
    /// Returns a dynamic script by its key
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="scriptKey">Script key</param>
    /// <param name="contentType">Content type</param>
    /// <param name="json">True to return JSON</param>
    public async static Task ReturnScript(HttpContext context, string scriptKey, string contentType, bool json)
    {
        IScriptContent scriptContent;
        try
        {
            scriptContent = context.RequestServices.GetRequiredService<IDynamicScriptManager>()
                .ReadScriptContent(scriptKey, json);
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
            Encoding = Encoding.UTF8
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

        var supportsBrotli = scriptContent.CanCompress &&
            context.Request.Headers["Accept-Encoding"].Any(x => x.Contains("br", StringComparison.Ordinal));

        var supportsGzip = !supportsBrotli && scriptContent.CanCompress && 
            context.Request.Headers["Accept-Encoding"].Any(x => x.Contains("gzip", StringComparison.Ordinal));

        byte[] contentBytes;
        if (supportsBrotli)
        {
            context.Response.Headers["Content-Encoding"] = "br";
            contentBytes = scriptContent.BrotliContent;
        }
        else if (supportsGzip)
        {
            context.Response.Headers["Content-Encoding"] = "gzip";
            contentBytes = scriptContent.CompressedContent;
        }
        else
            contentBytes = scriptContent.Content;

        await WriteWithIfModifiedSinceControl(context, contentBytes,
            scriptContent.Time);
    }

    /// <summary>
    /// Writes a file content to the response with modified since control
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="bytes">Content bytes</param>
    /// <param name="lastWriteTime">Last write time</param>
    /// <exception cref="ArgumentNullException">Context is null</exception>
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
