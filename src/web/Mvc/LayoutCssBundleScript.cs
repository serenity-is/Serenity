namespace Serenity.Web;

/// <summary>
/// A predefined dynamic CSS bundle that concatenates the layout CSS files
/// declared with <see cref="LayoutCssFileAttribute"/>.
/// It is registered under <see cref="ScriptName"/> at startup, so it can be
/// referenced from a bundle definition (e.g. <c>dynamic://CssBundle.LayoutCssFiles</c>)
/// or rendered with <c>@Html.Stylesheet("dynamic://CssBundle.LayoutCssFiles")</c>.
/// </summary>
/// <remarks>
/// Unlike <c>Html.LayoutCssFiles()</c>, the file set is resolved by
/// <see cref="ILayoutCssFileProvider.GetFiles"/> the first time the script is
/// generated and then cached, so it does not change dynamically when feature
/// toggles change at runtime. Use <c>Html.LayoutCssFiles()</c> when the file set
/// should follow feature toggles per request.
/// Initializes a new instance of the <see cref="LayoutCssBundleScript"/> class.
/// </remarks>
/// <param name="fileProvider">The layout CSS file provider.</param>
/// <param name="bundleBuilder">The CSS bundle builder.</param>
/// <exception cref="ArgumentNullException">One of the arguments is <c>null</c>.</exception>
public class LayoutCssBundleScript(ILayoutCssFileProvider fileProvider,
    ICssBundleBuilder bundleBuilder) : DynamicScript
{
    /// <summary>
    /// The predefined registration name of the layout CSS bundle.
    /// </summary>
    public const string ScriptName = "CssBundle.LayoutCssFiles";

    private readonly ILayoutCssFileProvider fileProvider = fileProvider
        ?? throw new ArgumentNullException(nameof(fileProvider));
    private readonly ICssBundleBuilder bundleBuilder = bundleBuilder
        ?? throw new ArgumentNullException(nameof(bundleBuilder));

    /// <inheritdoc/>
    public override string GetScript()
    {
        var files = fileProvider.GetFiles();
        if (files.Count == 0)
            return "";

        return bundleBuilder.BuildBundle("../", files).GetScript();
    }
}
