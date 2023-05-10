using Microsoft.Extensions.Options;

namespace Serenity.Web;

/// <summary>
/// Options for script bundling
/// </summary>
[DefaultSectionKey(SectionKey)]
public class ScriptBundlingOptions : IOptions<ScriptBundlingOptions>
{
    /// <summary>
    /// Default section key for this option class
    /// </summary>
    public const string SectionKey = "ScriptBundling";

    /// <summary>
    /// Creates a new instance of the class
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
    /// Gets / sets the list of bundles and their contents
    /// </summary>
    public Dictionary<string, string[]> Bundles { get; }

    /// <summary>
    /// Gets / sets if script bundling is enabled
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Gets / sets the minimization flag. When true, script files are minified
    /// and their minified versions are used in bundles etc.
    /// </summary>
    public bool? Minimize { get; set; }

    /// <summary>
    /// A list of relative paths to not minify
    /// </summary>
    public string[] NoMinimize { get; set; }

    /// <summary>
    /// Replacement dictionary for placeholders in bundle contents,
    /// like {Development} etc.
    /// </summary>
    public Dictionary<string, object> Replacements { get; }

    /// <summary>
    /// Should a ".min.js" file that exists in file system be used if available,
    /// instead of minifiying the file in memory.
    /// </summary>
    public bool? UseMinJS { get; set; }

    /// <summary>
    /// Returns this object.
    /// </summary>
    public ScriptBundlingOptions Value => this;
}
