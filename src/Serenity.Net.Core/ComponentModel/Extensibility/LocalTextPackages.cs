namespace Serenity.Web;

/// <summary>
/// Corresponds to LocalTextPackages section of the appsettings.json file
/// </summary>
[DefaultSectionKey(SectionKey)]
public class LocalTextPackages : Dictionary<string, string>
{
    /// <summary>
    /// Default section key
    /// </summary>
    public const string SectionKey = "LocalTextPackages";

    /// <summary>
    /// Default regex for set of texts included in Site package
    /// </summary>
    public static readonly Regex DefaultSitePackageIncludes = new(@"^(Columns|Controls|Db|Dialogs|Enums|Forms|Permission|Site|Validation)\.", RegexOptions.Compiled);

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public LocalTextPackages()
        : base(StringComparer.OrdinalIgnoreCase)
    {
        Add("Site", "");
    }
}