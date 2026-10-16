using Microsoft.Extensions.Primitives;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="ILayoutCssFileWatcher"/>, which uses
/// <see cref="IWebRootFileWatcher"/> change tokens (only available in
/// development) to invalidate the dynamic script when a file changes. Each
/// script name is only watched once.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LayoutCssFileWatcher"/> class.
/// </remarks>
/// <param name="fileWatcher">The web root file watcher.</param>
/// <param name="scriptManager">The dynamic script manager.</param>
/// <exception cref="ArgumentNullException">One of the arguments is <c>null</c>.</exception>
public class LayoutCssFileWatcher(IWebRootFileWatcher fileWatcher,
    IDynamicScriptManager scriptManager) : ILayoutCssFileWatcher, IDisposable
{
    private readonly IWebRootFileWatcher fileWatcher = fileWatcher ?? throw new ArgumentNullException(nameof(fileWatcher));
    private readonly IDynamicScriptManager scriptManager = scriptManager ?? throw new ArgumentNullException(nameof(scriptManager));
    private static readonly IChangeToken neverChangeToken = new CancellationChangeToken(CancellationToken.None);
    private readonly Lock @lock = new();
    private readonly HashSet<string> registered = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<IDisposable> subscriptions = [];

    /// <inheritdoc/>
    public void Watch(string scriptName, IReadOnlyList<string> appRelativeUrls)
    {
        if (string.IsNullOrEmpty(scriptName) ||
            appRelativeUrls == null ||
            appRelativeUrls.Count == 0)
            return;

        lock (@lock)
        {
            if (registered.Contains(scriptName))
                return;

            var any = false;
            foreach (var url in appRelativeUrls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;

                var webPath = url.StartsWith("~/", StringComparison.Ordinal) ? url[2..] : url;
                if (fileWatcher.GetChangeToken(webPath) == null)
                    continue;

                subscriptions.Add(ChangeToken.OnChange(
                    () => fileWatcher.GetChangeToken(webPath) ?? neverChangeToken,
                    () => scriptManager.Changed(scriptName)));

                any = true;
            }

            if (any)
                registered.Add(scriptName);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (@lock)
        {
            foreach (var subscription in subscriptions)
                subscription.Dispose();

            subscriptions.Clear();
            registered.Clear();
        }

        GC.SuppressFinalize(this);
    }
}
