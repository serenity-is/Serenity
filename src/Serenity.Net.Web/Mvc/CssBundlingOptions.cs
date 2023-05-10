using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Options for css bundling
/// </summary>
[DefaultSectionKey(SectionKey)]
public class CssBundlingOptions : IOptions<CssBundlingOptions>
{
    /// <summary>
    /// Default section key for this option class
    /// </summary>
    public const string SectionKey = "CssBundling";

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public CssBundlingOptions()
    {
        Bundles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        Enabled = false;
        Minimize = true;
        UseMinCSS = true;
        Replacements = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets / sets if CSS bundling is enabled
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets / sets the minimization flag. When true, CSS files are minified
    /// and their minified versions are used in bundles etc.
    /// </summary>
    public bool? Minimize { get; set; }

    /// <summary>
    /// Should a ".min.css" file that exists in file system be used if available,
    /// instead of minifiying the file in memory.
    /// </summary>
    public bool? UseMinCSS { get; set; }

    /// <summary>
    /// A list of relative paths to not minify
    /// </summary>
    public string[] NoMinimize { get; set; }

    /// <summary>
    /// Replacement dictionary for placeholders in bundle contents,
    /// like {Development} etc.
    /// </summary>
    public Dictionary<string, object> Replacements { get; set; }

    /// <summary>
    /// Gets / sets the list of bundles and their contents
    /// </summary>
    public Dictionary<string, string[]> Bundles { get; set; }

    /// <summary>
    /// Returns this object.
    /// </summary>
    public CssBundlingOptions Value => this;
}
