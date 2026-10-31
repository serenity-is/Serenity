using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Serenity.Web;

/// <summary>
/// A file provider that exposes the contents of another file provider under
/// a path prefix, e.g. files of a package's wwwroot under "/Serenity.Corelib".
/// </summary>
/// <param name="prefix">Path prefix, like "Serenity.Corelib" or "/Serenity.Corelib"</param>
/// <param name="inner">Provider for the files under the prefix</param>
public sealed class PrefixedFileProvider(string prefix, IFileProvider inner) : IFileProvider
{
    private readonly string prefix = "/" + (prefix ?? throw new ArgumentNullException(nameof(prefix))).Trim('/');
    private readonly IFileProvider inner = inner ?? throw new ArgumentNullException(nameof(inner));

    /// <summary>
    /// Gets the path prefix, with a leading slash and no trailing slash.
    /// </summary>
    public string Prefix => prefix;

    /// <summary>
    /// Gets the inner file provider.
    /// </summary>
    public IFileProvider Inner => inner;

    private bool TryGetInnerPath(string subpath, out string innerPath)
    {
        innerPath = null!;
        if (string.IsNullOrEmpty(subpath))
            return false;

        if (subpath[0] != '/')
            subpath = "/" + subpath;

        if (!subpath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        if (subpath.Length == prefix.Length)
        {
            innerPath = "/";
            return true;
        }

        if (subpath[prefix.Length] != '/')
            return false;

        innerPath = subpath[prefix.Length..];
        return true;
    }

    /// <inheritdoc/>
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return TryGetInnerPath(subpath, out var innerPath) ?
            inner.GetDirectoryContents(innerPath) :
            NotFoundDirectoryContents.Singleton;
    }

    /// <inheritdoc/>
    public IFileInfo GetFileInfo(string subpath)
    {
        return TryGetInnerPath(subpath, out var innerPath) && innerPath.Length > 1 ?
            inner.GetFileInfo(innerPath) :
            new NotFoundFileInfo(subpath);
    }

    /// <inheritdoc/>
    public IChangeToken Watch(string filter)
    {
        return TryGetInnerPath(filter, out var innerPath) ?
            inner.Watch(innerPath) :
            NullChangeToken.Singleton;
    }
}
