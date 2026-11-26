using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web.Middleware;

/// <summary>
/// Middleware that serves the files under <c>esm</c> folders only to users who may load them,
/// see <see cref="EsmAccessControlOptions"/>. It must be placed before the static files middleware.
/// Denied files get a 404 response, so that it does not tell whether the file exists.
/// </summary>
/// <param name="next">The next middleware</param>
/// <param name="policy">The access policy</param>
public class EsmAccessMiddleware(RequestDelegate next, EsmAccessPolicy policy)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly EsmAccessPolicy policy = policy ?? throw new ArgumentNullException(nameof(policy));

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!HttpMethods.IsGet(context.Request.Method) &&
            !HttpMethods.IsHead(context.Request.Method))
        {
            await next(context);
            return;
        }

        var access = policy.GetAccess(context.Request.Path.Value);
        switch (access.Kind)
        {
            case EsmFileAccessKind.Hidden:
                NotFound(context);
                return;

            case EsmFileAccessKind.Restricted:
                // static files are served before authentication runs, so authenticate here
                await EnsureAuthenticated(context);
                var permissions = context.RequestServices.GetService<IPermissionService>();
                if (permissions is null ||
                    !access.Permissions.Any(permissions.HasPermission))
                {
                    NotFound(context);
                    return;
                }

                // a shared cache (e.g. a CDN or proxy) must not serve it to other users
                context.Response.OnStarting(() =>
                {
                    MakePrivate(context.Response.Headers);
                    return Task.CompletedTask;
                });
                break;
        }

        await next(context);
    }

    private static async Task EnsureAuthenticated(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
            return;

        try
        {
            var result = await context.AuthenticateAsync();
            if (result.Succeeded && result.Principal is not null)
                context.User = result.Principal;
        }
        catch (InvalidOperationException)
        {
            // no authentication services or default scheme, e.g. the user stays anonymous
        }
    }

    private static void NotFound(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.Headers.CacheControl = "private, no-store";
    }

    internal static void MakePrivate(IHeaderDictionary headers)
    {
        var cacheControl = headers.CacheControl.ToString();
        if (string.IsNullOrWhiteSpace(cacheControl))
        {
            headers.CacheControl = "private";
            return;
        }

        var directives = cacheControl.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.Equals(x, "public", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!directives.Any(x => string.Equals(x, "private", StringComparison.OrdinalIgnoreCase)))
            directives.Insert(0, "private");

        headers.CacheControl = string.Join(", ", directives);
    }
}
