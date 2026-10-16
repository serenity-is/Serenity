using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace Serenity.Web;

/// <summary>
/// Default implementation of <see cref="IWebRootFileWatcher"/>. It reuses
/// physical file providers rooted at <c>wwwroot</c> / <c>staticwebassets</c> so
/// that watching multiple files in the same directory does not create a new
/// provider each time. Watching is only done in development.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WebRootFileWatcher"/> class.
/// </remarks>
/// <param name="hostEnvironment">The web host environment.</param>
/// <exception cref="ArgumentNullException"><paramref name="hostEnvironment"/> is <c>null</c>.</exception>
public class WebRootFileWatcher(IWebHostEnvironment hostEnvironment) : IWebRootFileWatcher, IDisposable
{
    private readonly IWebHostEnvironment hostEnvironment = hostEnvironment
        ?? throw new ArgumentNullException(nameof(hostEnvironment));
    private readonly Lock @lock = new();
    private static readonly IChangeToken neverChangeToken = new CancellationChangeToken(CancellationToken.None);
    private readonly Dictionary<string, IFileProvider> watchFileProviders = new(
        OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    private readonly HashSet<string> registeredCallbackKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<IDisposable> subscriptions = [];

    private static bool IsSameOrChildDirectory(string root, string path)
    {
        var relative = System.IO.Path.GetRelativePath(root, path);
        return !System.IO.Path.IsPathRooted(relative) &&
            relative != ".." &&
            !relative.StartsWith(".." + System.IO.Path.DirectorySeparatorChar, StringComparison.Ordinal);
    }

    internal int SubscriptionCount => subscriptions.Count;

    internal int RegisteredCallbackKeyCount => registeredCallbackKeys.Count;

    /// <inheritdoc/>
    public IChangeToken? GetChangeToken(string webRelativePath)
    {
        if (string.IsNullOrEmpty(webRelativePath) ||
            !hostEnvironment.IsDevelopment())
            return null;

        try
        {
            var physicalPath = hostEnvironment.WebRootFileProvider
                .GetFileInfo(webRelativePath).PhysicalPath;

            if (string.IsNullOrEmpty(physicalPath) ||
                !System.IO.File.Exists(physicalPath))
                return null;

            lock (@lock)
                return GetChangeTokenCore(physicalPath);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public void RegisterCallback(string key, IEnumerable<string> appRelativePaths, Action callback)
    {
        if (string.IsNullOrEmpty(key) ||
            appRelativePaths == null ||
            callback == null ||
            !hostEnvironment.IsDevelopment())
            return;

        lock (@lock)
        {
            if (!registeredCallbackKeys.Add(key))
                return;

            var any = false;
            foreach (var path in appRelativePaths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;

                var webPath = path.StartsWith("~/", StringComparison.Ordinal) ? path[2..] : path;
                if (GetChangeToken(webPath) == null)
                    continue;

                subscriptions.Add(ChangeToken.OnChange(
                    () => GetChangeToken(webPath) ?? neverChangeToken,
                    callback));

                any = true;
            }

            if (!any)
                registeredCallbackKeys.Remove(key);
        }
    }

    private IChangeToken? GetChangeTokenCore(string physicalPath)
    {
        var directory = PathHelper.ToUrl(System.IO.Path.GetDirectoryName(physicalPath));
        if (string.IsNullOrEmpty(directory))
            return null;

        var wwwroot = directory.IndexOf("/wwwroot/", StringComparison.Ordinal);
        if (wwwroot >= 0)
            directory = directory[0..(wwwroot + 8)];
        else
        {
            var staticwebassets = directory.IndexOf("/staticwebassets/", StringComparison.Ordinal);
            if (staticwebassets >= 0)
                directory = directory[0..(staticwebassets + 16)];
        }

        var watchRoot = System.IO.Path.GetFullPath(PathHelper.ToPath(directory)!);
        physicalPath = System.IO.Path.GetFullPath(physicalPath);
        IFileProvider? provider = null;
        string? providerRoot = null;

        if (watchFileProviders.TryGetValue(watchRoot, out provider))
            providerRoot = watchRoot;
        else
        {
            foreach (var pair in watchFileProviders)
            {
                if (IsSameOrChildDirectory(pair.Key, watchRoot) &&
                    (providerRoot == null || pair.Key.Length > providerRoot.Length))
                {
                    provider = pair.Value;
                    providerRoot = pair.Key;
                }
            }

            if (provider == null)
            {
                provider = new PhysicalFileProvider(watchRoot);
                providerRoot = watchRoot;
                watchFileProviders[watchRoot] = provider;
            }
        }

        var filter = PathHelper.ToUrl(System.IO.Path.GetRelativePath(providerRoot!, physicalPath));
        return provider.Watch(filter!);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (@lock)
        {
            foreach (var subscription in subscriptions)
                subscription.Dispose();
            subscriptions.Clear();
            registeredCallbackKeys.Clear();

            foreach (var provider in watchFileProviders.Values)
            {
                if (provider is IDisposable disposable)
                    disposable.Dispose();
            }

            watchFileProviders.Clear();
        }

        GC.SuppressFinalize(this);
    }
}
