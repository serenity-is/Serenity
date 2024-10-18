
using Microsoft.AspNetCore.Http;

namespace Serenity.Extensions;

/// <summary>
/// Default implementation for <see cref="ISiteAbsoluteUrl"/>
/// </summary>
public class SiteAbsoluteUrl(
    IOptionsMonitor<EnvironmentSettings> environmentSettings,
    IHttpContextAccessor httpContextAccessor = null) : ISiteAbsoluteUrl
{
    protected readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    protected readonly IOptionsMonitor<EnvironmentSettings> environmentSettings = environmentSettings ??
            throw new ArgumentNullException(nameof(environmentSettings));

    protected virtual string TryGetInternalUrl()
    {
        var url = environmentSettings.CurrentValue?.SiteInternalUrl;
        if (string.IsNullOrEmpty(url))
            return GetExternalUrl();

        return url;
    }

    public virtual string GetInternalUrl()
    {
        var url = TryGetInternalUrl();

        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Can't determine the internal URL for the web site. " +
                "Please set one of EnvironmentSettings:SiteExternalUrl or " +
                "EnvironmentSettings:SiteInternalUrl in appsettings.json");

        return url;
    }

    protected virtual string TryGetExternalUrl()
    {
        return httpContextAccessor?.HttpContext?.Request?.GetBaseUri()?.AbsoluteUri ??
            environmentSettings.CurrentValue?.SiteExternalUrl;
    }

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