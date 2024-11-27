
namespace Serenity.Extensions;

[DefaultSectionKey(SectionKey)]
public class EnvironmentSettings
{
    public const string SectionKey = "EnvironmentSettings";

    /// <summary>
    /// This was a flag used by Serenity Demo, but as it is using C# preprocessor
    /// directives now, this flag is obsolete
    /// </summary>
    public bool IsPublicDemo { get; set; }

    /// <summary>
    /// The externally accessible, public url of the web site, used for link generation in emails etc.
    /// The default implementation for ISiteAbsoluteUrl.GetExternalUrl() uses this value only when
    /// an HTTP request is not available
    /// </summary>
    public string SiteExternalUrl { get; set; }


    /// <summary>
    /// The internally accessible, local network url of the web site, used for URL generation for report
    /// tools to call back into the web site locally. If set, the default implementation for
    /// ISiteAbsoluteUrl.GetInternalUrl() will prefer this over the current request's base uri,
    /// and external URL setting.
    /// </summary>
    public string SiteInternalUrl { get; set; }

    /// <summary>
    /// Optional markup to inject into head, can be used for analytics tags etc.
    /// </summary>
    public string InjectMarkupToHead { get; set; }
}