using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Options for script bundling.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class ScriptBundlingOptions : IOptions<ScriptBundlingOptions>
{
    /// <summary>
    /// The default section key for this option class.
    /// </summary>
    public const string SectionKey = "ScriptBundling";

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptBundlingOptions"/> class.
    /// </summary>
    public ScriptBundlingOptions()
    {
        Bundles = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        Enabled = false;
        Minimize = true;
        UseMinJS = true;
        Replacements = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the list of bundles and their contents.
    /// </summary>
    public Dictionary<string, string[]> Bundles { get; }

    /// <summary>
    /// Gets or sets whether script bundling is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets or sets the minimization flag. When <c>true</c>, script files are minified
    /// and their minified versions are used in bundles.
    /// </summary>
    public bool? Minimize { get; set; }

    /// <summary>
    /// Gets or sets a list of relative paths to not minify.
    /// </summary>
    public string[] NoMinimize { get; set; }

    /// <summary>
    /// Gets the replacement dictionary for placeholders in bundle contents,
    /// like <c>{Development}</c>.
    /// </summary>
    public Dictionary<string, object> Replacements { get; }

    /// <summary>
    /// Gets or sets whether a <c>.min.js</c> file that exists in the file system
    /// should be used if available, instead of minifying the file in memory.
    /// </summary>
    public bool? UseMinJS { get; set; }

    /// <summary>
    /// Returns this object.
    /// </summary>
    public ScriptBundlingOptions Value => this;
}
