namespace Serenity.Web;

/// <summary>
/// A predefined dynamic script bundle that concatenates the layout JS files
/// declared with <see cref="LayoutScriptFileAttribute"/>.
/// It is registered under <see cref="ScriptName"/> at startup, so it can be
/// referenced from a bundle definition (e.g. <c>dynamic://Bundle.LayoutScriptFiles</c>)
/// or rendered with <c>@Html.Script("dynamic://Bundle.LayoutScriptFiles")</c>.
/// </summary>
/// <remarks>
/// Unlike <c>Html.LayoutScriptFiles()</c>, the file set is resolved by
/// <see cref="ILayoutScriptFileProvider.GetFiles"/> the first time the script is
/// generated and then cached, so it does not change dynamically when feature
/// toggles change at runtime. Use <c>Html.LayoutScriptFiles()</c> when the file set
/// should follow feature toggles per request.
/// Initializes a new instance of the <see cref="LayoutScriptBundleScript"/> class.
/// </remarks>
/// <param name="fileProvider">The layout script file provider.</param>
/// <param name="bundleBuilder">The script bundle builder.</param>
/// <exception cref="ArgumentNullException">One of the arguments is <c>null</c>.</exception>
public class LayoutScriptBundleScript(ILayoutScriptFileProvider fileProvider,
    IScriptBundleBuilder bundleBuilder) : DynamicScript
{
    /// <summary>
    /// The predefined registration name of the layout script bundle.
    /// </summary>
    public const string ScriptName = "Bundle.LayoutScriptFiles";

    private readonly ILayoutScriptFileProvider fileProvider = fileProvider
        ?? throw new ArgumentNullException(nameof(fileProvider));
    private readonly IScriptBundleBuilder bundleBuilder = bundleBuilder
        ?? throw new ArgumentNullException(nameof(bundleBuilder));

    /// <inheritdoc/>
    public override string GetScript()
    {
        var files = fileProvider.GetFiles();
        if (files.Count == 0)
            return "";

        return bundleBuilder.BuildBundle(files).GetScript();
    }
}
