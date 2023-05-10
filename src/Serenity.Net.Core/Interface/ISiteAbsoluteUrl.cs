
namespace Serenity.Abstractions;

/// <summary>
/// Interface to access absolute base URL for current site,
/// including the PathBase. Useful for reporting callbacks, 
/// or e-mails etc.
/// </summary>
public interface ISiteAbsoluteUrl
{
    /// <summary>
    /// Returns the absolute internal/local url of the current site,
    /// used to generate URLs for report tools like WKHTML, Puppeeteer etc.
    /// The default implementation builds it from one of these in this order: 
    /// 1) EnvironmentSettings:SiteInternalUrl setting, 
    /// 2) The current http request base url if available, 
    /// 3) EnvironmentSettings:SiteExternalUrl setting
    /// </summary>
    string GetInternalUrl();

    /// <summary>
    /// Returns the absolute external/public url of the current site,
    /// used to generate links for activation e-mails etc. 
    /// The default implementation builds it from one of these in this order:
    /// 1) The current http request base url if available, 
    /// 2) EnvironmentSettings:SiteExternalUrl setting
    /// </summary>
    string GetExternalUrl();
}