namespace Serenity.Web;

/// <summary>
/// Declares a JS file that should be included in the layout on every page,
/// usually rendered with <c>@Html.LayoutScriptFiles()</c>. This is intended for
/// dynamically loaded libraries that need their scripts to be available on all
/// pages, without depending on a page specific entry point.
/// The declared files are also exposed as a predefined dynamic bundle named
/// <see cref="LayoutScriptBundleScript.ScriptName"/>, which can be referenced from
/// a bundle definition (e.g. <c>dynamic://Bundle.LayoutScriptFiles</c>). Note that
/// the predefined bundle is generated once and cached, so it does not follow
/// feature toggles dynamically; use <c>@Html.LayoutScriptFiles()</c> for that.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class LayoutScriptFileAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="path">The app relative (<c>~/</c>) JS file path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c>.</exception>
    public LayoutScriptFileAttribute(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    /// <summary>
    /// Gets the app relative (<c>~/</c>) JS file path.
    /// </summary>
    public string Path { get; }

    private bool? module;

    /// <summary>
    /// Gets or sets a value indicating whether the file is an ES module, which is rendered with its own
    /// <c>type="module"</c> script tag and never concatenated to a bundle, as its imports are relative to
    /// its own URL. Defaults to <c>true</c> for files under an <c>esm</c> folder and <c>.mjs</c> files.
    /// </summary>
    public bool Module
    {
        get => module ?? (Path.Contains("/esm/", StringComparison.OrdinalIgnoreCase) ||
            Path.EndsWith(".mjs", StringComparison.OrdinalIgnoreCase));
        set => module = value;
    }

    /// <summary>
    /// Gets or sets the order among the included files. Files with a lower
    /// order are included first. Defaults to <c>0</c>.
    /// </summary>
    public decimal Order { get; set; }

    /// <summary>
    /// Gets or sets the set of feature toggles this file depends on. When set,
    /// the file is only included if all (or any, see
    /// <see cref="RequireAnyFeature"/>) of the features are enabled.
    /// </summary>
    public string[]? RequireFeatures { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any of the
    /// <see cref="RequireFeatures"/> should be enabled instead of all of them.
    /// </summary>
    public bool RequireAnyFeature { get; set; }
}
