using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;

namespace Serenity.Web;

/// <summary>
/// A file provider that only exposes the files of another file provider that a filter accepts,
/// e.g. by the include / exclude patterns of <see cref="StaticAssetPatternFilter"/>.
/// Directories are always exposed, their contents are filtered.
/// </summary>
/// <param name="inner">Inner file provider</param>
/// <param name="isMatch">Filter that receives a path relative to the provider root, like "css/site.css"</param>
public sealed class FilteredFileProvider(IFileProvider inner, Func<string, bool> isMatch) : IFileProvider
{
    private readonly IFileProvider inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly Func<string, bool> isMatch = isMatch ?? throw new ArgumentNullException(nameof(isMatch));

    /// <inheritdoc/>
    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var contents = inner.GetDirectoryContents(subpath);
        if (!contents.Exists)
            return contents;

        var directory = (subpath ?? "").Replace('\\', '/').Trim('/');
        return new FilteredDirectoryContents(contents, file =>
            file.IsDirectory || isMatch(directory.Length == 0 ? file.Name : directory + "/" + file.Name));
    }

    /// <inheritdoc/>
    public IFileInfo GetFileInfo(string subpath)
    {
        var path = (subpath ?? "").Replace('\\', '/').TrimStart('/');
        if (path.Length == 0 || !isMatch(path))
            return new NotFoundFileInfo(subpath ?? "");

        return inner.GetFileInfo(subpath!);
    }

    /// <inheritdoc/>
    public IChangeToken Watch(string filter)
    {
        return inner.Watch(filter);
    }

    private sealed class FilteredDirectoryContents(IDirectoryContents contents, Func<IFileInfo, bool> predicate) : IDirectoryContents
    {
        public bool Exists => contents.Exists;

        public IEnumerator<IFileInfo> GetEnumerator() => contents.Where(predicate).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
