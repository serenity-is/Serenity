using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Options for CSS bundling.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class CssBundlingOptions : IOptions<CssBundlingOptions>
{
    /// <summary>
    /// The default section key for this option class.
    /// </summary>
    public const string SectionKey = "CssBundling";

    /// <summary>
    /// Initializes a new instance of the <see cref="CssBundlingOptions"/> class.
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
    /// Gets or sets whether CSS bundling is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the minimization flag. When <c>true</c>, CSS files are minified
    /// and their minified versions are used in bundles.
    /// </summary>
    public bool? Minimize { get; set; }

    /// <summary>
    /// Gets or sets whether a <c>.min.css</c> file that exists in the file system
    /// should be used if available, instead of minifying the file in memory.
    /// </summary>
    public bool? UseMinCSS { get; set; }

    /// <summary>
    /// Gets or sets a list of relative paths to not minify.
    /// </summary>
    public string[] NoMinimize { get; set; }

    /// <summary>
    /// Gets or sets the replacement dictionary for placeholders in bundle contents,
    /// like <c>{Development}</c>.
    /// </summary>
    public Dictionary<string, object> Replacements { get; set; }

    /// <summary>
    /// Gets or sets the list of bundles and their contents.
    /// </summary>
    public Dictionary<string, string[]> Bundles { get; set; }

    /// <summary>
    /// Returns this object.
    /// </summary>
    public CssBundlingOptions Value => this;
}
