using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// An abstraction for a content hash cache, which contains hashes of web servable files.
/// </summary>
public interface IContentHashCache
{
    /// <summary>
    /// Resolves a relative path.
    /// </summary>
    /// <param name="pathBase">The path base.</param>
    /// <param name="contentPath">The content path.</param>
    /// <returns>The resolved path.</returns>
    string ResolvePath(PathString pathBase, string contentPath);

    /// <summary>
    /// Resolves a relative path with its hash.
    /// </summary>
    /// <param name="pathBase">The path base.</param>
    /// <param name="contentUrl">The content URL.</param>
    /// <returns>The resolved URL with its hash.</returns>
    string ResolveWithHash(PathString pathBase, string contentUrl);

    /// <summary>
    /// Raises the scripts changed event, causing the cache to be cleared.
    /// </summary>
    void ScriptsChanged();
}