namespace Serenity.Web;

/// <summary>
/// A predefined dynamic CSS bundle that concatenates the layout CSS files
/// declared with <see cref="LayoutCssFileAttribute"/>.
/// It is registered under <see cref="ScriptName"/> at startup, so it can be
/// referenced from a bundle definition (e.g. <c>dynamic://CssBundle.LayoutCssFiles</c>)
/// or rendered with <c>@Html.Stylesheet("dynamic://CssBundle.LayoutCssFiles")</c>.
/// </summary>
/// <remarks>
/// The files are the ones <see cref="ILayoutCssFileProvider.GetFiles"/> returns for the current
/// request, e.g. filtered by feature toggles and the permissions of the current user. A hash of the
/// file list is the <see cref="CacheSuffix"/>, so each distinct file set is generated and cached
/// separately, and has its own content hash in urls. The bundle itself is public, as it only contains
/// the files the requesting user may load.
/// Initializes a new instance of the <see cref="LayoutCssBundleScript"/> class.
/// </remarks>
/// <param name="fileProvider">The layout CSS file provider.</param>
/// <param name="bundleBuilder">The CSS bundle builder.</param>
/// <param name="filter">An optional filter applied to the file urls, e.g. to exclude the files of an assembly.</param>
/// <exception cref="ArgumentNullException">One of the arguments is <c>null</c>.</exception>
public class LayoutCssBundleScript(ILayoutCssFileProvider fileProvider,
    ICssBundleBuilder bundleBuilder, Func<string, bool>? filter = null) : DynamicScript, ICacheSuffix
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
    public string CacheSuffix => LayoutFileListHash.Compute(GetFiles());

    private IReadOnlyList<string> GetFiles()
    {
        var files = fileProvider.GetFiles();
        return filter == null ? files : [.. files.Where(filter)];
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        var files = GetFiles();
        if (files.Count == 0)
            return "";

        return bundleBuilder.BuildBundle("../", files).GetScript();
    }
}
