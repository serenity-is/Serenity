using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register the account rate limiter
/// </summary>
public static class AccountRateLimiterServiceCollectionExtensions
{
    /// <summary>
    /// Adds a global rate limiter that limits POST requests to account actions like
    /// login, forgot password and sign up per client IP address and path, see
    /// <see cref="AccountRateLimitOptions"/>. Other requests are not limited.
    /// Call <c>app.UseRateLimiter()</c> in the request pipeline to activate it.
    /// If the application is behind a proxy, make sure forwarded headers are configured,
    /// otherwise all clients share the IP address of the proxy.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddAccountRateLimiter(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(GetPartition);
            options.OnRejected = OnRejected;
        });

        return services;
    }

    private static RateLimitPartition<string> GetPartition(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
            return RateLimitPartition.GetNoLimiter(string.Empty);

        var options = context.RequestServices.GetRequiredService<IOptions<AccountRateLimitOptions>>().Value;
        if (!options.Enabled ||
            options.Paths is null ||
            options.Paths.Count == 0)
            return RateLimitPartition.GetNoLimiter(string.Empty);

        var path = context.Request.Path.Value?.TrimEnd('/');
        if (string.IsNullOrEmpty(path) ||
            !TryGetPathOptions(options.Paths, path, out var pathOptions))
            return RateLimitPartition.GetNoLimiter(string.Empty);

        var permitLimit = pathOptions?.PermitLimit ?? options.PermitLimit;
        if (permitLimit <= 0)
            return RateLimitPartition.GetNoLimiter(string.Empty);

        var windowSeconds = Math.Max(1, pathOptions?.WindowSeconds ?? options.WindowSeconds);
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // limit and window are part of the key, so a configuration change applies to new requests
        var key = ip + "|" + path.ToLowerInvariant() + "|" + permitLimit.ToString(CultureInfo.InvariantCulture) + "/" +
            windowSeconds.ToString(CultureInfo.InvariantCulture);

        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueLimit = 0
        });
    }

    private static bool TryGetPathOptions(Dictionary<string, AccountRateLimitPathOptions> paths, string path,
        out AccountRateLimitPathOptions? pathOptions)
    {
        // not a dictionary lookup, as configured keys may have a trailing slash, and a dictionary
        // assigned in code may not be case-insensitive
        foreach (var pair in paths)
        {
            if (string.Equals(pair.Key.Trim().TrimEnd('/'), path, StringComparison.OrdinalIgnoreCase))
            {
                pathOptions = pair.Value;
                return true;
            }
        }

        pathOptions = null;
        return false;
    }

    private static async ValueTask OnRejected(OnRejectedContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds))
                .ToString(CultureInfo.InvariantCulture);

        var localizer = context.HttpContext.RequestServices.GetService<ITextLocalizer>();
        var message = AccountRateLimitTexts.TooManyRequests.ToString(localizer);

        // account actions are called via serviceCall / serviceFetch, which displays the error message
        response.ContentType = "application/json; charset=utf-8";
        await response.WriteAsync(JSON.Stringify(new ServiceResponse
        {
            Error = new ServiceError
            {
                Code = "TooManyRequests",
                Message = message
            }
        }), cancellationToken);
    }
}
