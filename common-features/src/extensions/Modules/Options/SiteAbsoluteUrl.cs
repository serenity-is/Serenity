
using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Default implementation for <see cref="ISiteAbsoluteUrl"/>
/// </summary>
public class SiteAbsoluteUrl(
    IOptionsMonitor<EnvironmentSettings> environmentSettings,
    IHttpContextAccessor httpContextAccessor = null) : ISiteAbsoluteUrl
{
    /// <summary>
    /// The HTTP context accessor used to resolve the current request's base URI.
    /// </summary>
    protected readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// The environment settings used to resolve the internal and external URLs.
    /// </summary>
    protected readonly IOptionsMonitor<EnvironmentSettings> environmentSettings = environmentSettings ??
            throw new ArgumentNullException(nameof(environmentSettings));

    /// <summary>
    /// Tries to get the internal URL, falling back to the external URL when no internal URL is configured.
    /// </summary>
    protected virtual string TryGetInternalUrl()
    {
        var url = environmentSettings.CurrentValue?.SiteInternalUrl;
        if (string.IsNullOrEmpty(url))
            return GetExternalUrl();

        return url;
    }

    /// <summary>
    /// Gets the internal URL of the web site, throwing if it cannot be determined.
    /// </summary>
    public virtual string GetInternalUrl()
    {
        var url = TryGetInternalUrl();

        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Can't determine the internal URL for the web site. " +
                "Please set one of EnvironmentSettings:SiteExternalUrl or " +
                "EnvironmentSettings:SiteInternalUrl in appsettings.json");

        return url;
    }

    /// <summary>
    /// Tries to get the external URL from the current request or the configured site external URL.
    /// </summary>
    protected virtual string TryGetExternalUrl()
    {
        return httpContextAccessor?.HttpContext?.Request?.GetBaseUri()?.AbsoluteUri ??
            environmentSettings.CurrentValue?.SiteExternalUrl;
    }

    /// <summary>
    /// Gets the external URL of the web site, throwing if it cannot be determined.
    /// </summary>
    public virtual string GetExternalUrl()
    {
        var url = TryGetExternalUrl();

        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Can't determine the public URL for the web site. " +
                "Please set one of EnvironmentSettings:SiteExternalUrl " +
                " in appsettings.json");

        return url;
    }
}