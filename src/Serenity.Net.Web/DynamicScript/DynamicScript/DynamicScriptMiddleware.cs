using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Serenity.Services;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Serenity.Data;
using Serenity.PropertyGrid;

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
            scriptKey = scriptKey.Substring(dynJSPath.Length);

            var contentType = "text/javascript";
            if (scriptKey.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                scriptKey = scriptKey.Substring(0, scriptKey.Length - 3);
            else if (scriptKey.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "text/css";
                scriptKey = scriptKey.Substring(0, scriptKey.Length - 4);
            }

            return ReturnScript(context, scriptKey, contentType);
        }

        public async static Task ReturnScript(HttpContext context, string scriptKey, string contentType)
        {
            IDynamicScriptData dynamicScript;
            try
            {
                dynamicScript = context.RequestServices.GetRequiredService<IDynamicScriptManager>().GetScriptData(scriptKey);
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

            if (dynamicScript == null)
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

            var supportsGzip = dynamicScript.CompressedBytes != null &&
                context.Request.Headers["Accept-Encoding"].Any(x => x.IndexOf("gzip") >= 0);

            byte[] contentBytes;
            if (supportsGzip)
            {
                context.Response.Headers["Content-Encoding"] = "gzip";
                contentBytes = dynamicScript.CompressedBytes;
            }
            else
                contentBytes = dynamicScript.UncompressedBytes;

            await WriteWithIfModifiedSinceControl(context, contentBytes, dynamicScript.Time);
        }

        public async static Task WriteWithIfModifiedSinceControl(HttpContext context, byte[] bytes, DateTime lastWriteTime)
        {
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
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    public static class DynamicScriptMiddlewareExtensions
    {
        public static IApplicationBuilder UseDynamicScripts(this IApplicationBuilder builder, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var scriptManager = builder.ApplicationServices.GetRequiredService<IDynamicScriptManager>();
            var connections = builder.ApplicationServices.GetRequiredService<IConnectionFactory>();
            var propertyRegistry = builder.ApplicationServices.GetRequiredService<IPropertyItemRegistry>();

            DynamicScriptRegistration.Initialize(scriptManager, connections, assemblies);
            LookupScriptRegistration.RegisterLookupScripts(scriptManager, assemblies);
            DistinctValuesRegistration.RegisterDistinctValueScripts(scriptManager, assemblies);
            FormScriptRegistration.RegisterFormScripts(scriptManager, propertyRegistry, assemblies);
            ColumnsScriptRegistration.RegisterColumnsScripts(scriptManager, propertyRegistry, assemblies);

            var hostEnvironment = builder.ApplicationServices.GetService<IWebHostEnvironment>();
            new TemplateScriptRegistrar()
                .Initialize(scriptManager, new[] 
                {
                    System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Views" + System.IO.Path.DirectorySeparatorChar + "Templates"),
                    System.IO.Path.Combine(hostEnvironment.ContentRootPath, "Modules")
                }, watchForChanges: true);

            var scriptsPath = System.IO.Path.Combine(hostEnvironment.WebRootPath, "Scripts");
            var scriptWatcher = new FileWatcher(scriptsPath, "*.js");
            scriptWatcher.Changed += (name) =>
            {
                builder.ApplicationServices.GetService<IScriptBundleManager>()?.ScriptsChanged();
                builder.ApplicationServices.GetService<IContentHashCache>()?.ScriptsChanged();
            };

            var contentPath = System.IO.Path.Combine(hostEnvironment.WebRootPath, "Content");
            var cssWatcher = new FileWatcher(contentPath, "*.css");
            scriptWatcher.Changed += (name) =>
            {
                builder.ApplicationServices.GetService<ICssBundleManager>()?.CssChanged();
                builder.ApplicationServices.GetService<IContentHashCache>()?.ScriptsChanged();
            };

            return builder.UseMiddleware<DynamicScriptMiddleware>();
        }
    }
}
