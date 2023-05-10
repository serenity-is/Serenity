using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serenity.Localization;
using Serenity.Web;

namespace SerenityIs.Administration;

/// <summary>
/// Local text data script to access local texts from an external app like mobile
/// </summary>
[DataScript("LocalText", CacheDuration = 3600)]
public class LocalTextDataScript : DataScript<IDictionary<string, string>>, ICacheSuffix
{
    private readonly ILocalTextRegistry localTextRegistry;
    private readonly IOptions<LocalTextPackages> localTextPackages;
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="localTextRegistry">Local text registry</param>
    /// <param name="localTextPackages">Package list</param>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    public LocalTextDataScript(ILocalTextRegistry localTextRegistry, IOptions<LocalTextPackages> localTextPackages, IHttpContextAccessor httpContextAccessor)
    {
        this.localTextRegistry = localTextRegistry ?? throw new ArgumentNullException(nameof(localTextRegistry));
        this.localTextPackages = localTextPackages ?? throw new ArgumentNullException(nameof(localTextPackages));
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    
    /// <inheritdoc/>
    public string CacheSuffix => GetLanguageId(out string packageId) + ":" + packageId;

    private static readonly Regex LanguageIDRegex = new(@"^[a-z]{2}(-[A-Z]{2})?$");

    private string GetLanguageId(out string packageId)
    {
        string languageId = httpContextAccessor?.HttpContext?.Request?.Query["lang"];
        packageId = ((string)httpContextAccessor?.HttpContext?.Request?.Query["pack"]) ?? "Site";

        if (localTextPackages?.Value.ContainsKey(packageId) != true)
            packageId = "Site";

        if (!string.IsNullOrEmpty(languageId) &&
            LanguageIDRegex.IsMatch(languageId))
        {
            return languageId;
        }

        return CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
    }

    /// <summary>
    /// Gets a local text package as a dictionary
    /// </summary>
    /// <param name="registry">Text registry</param>
    /// <param name="includes">Includes regex</param>
    /// <param name="languageId">Language ID</param>
    /// <param name="isPending">True to include pending text</param>
    /// <param name="packageId">Package ID</param>
    /// <exception cref="ArgumentNullException">Registry is null</exception>
    public static IDictionary<string, string> GetPackageData(ILocalTextRegistry registry,
        string includes, string languageId, bool isPending, string packageId = null)
    {
        if (registry == null)
            throw new ArgumentNullException(nameof(registry));

        var result = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(includes))
            includes = "$^";

        var regex = new Regex(includes, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var texts = registry is LocalTextRegistry ltr ? 
            ltr.GetAllAvailableTextsInLanguage(languageId, isPending) : new Dictionary<string, string>();

        foreach (var pair in texts)
            if (regex.IsMatch(pair.Key) ||
                (packageId == "Site" && LocalTextPackages.DefaultSitePackageIncludes.IsMatch(pair.Key)))
                result[pair.Key] = pair.Value;

        return result;
    }

    /// <inheritdoc/>
    protected override IDictionary<string, string> GetData()
    {
        var languageId = GetLanguageId(out var packageId);

        if (localTextPackages?.Value.TryGetValue(packageId, out var includes) != true)
            includes = "^$";

        return GetPackageData(localTextRegistry, includes, languageId, false, packageId);
    }
}
