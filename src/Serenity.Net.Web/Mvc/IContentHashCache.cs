using Microsoft.AspNetCore.Http;

namespace Serenity.Web;

/// <summary>
/// An abstraction for content hash cache, which contains hashes of web servable files.
/// </summary>
public interface IContentHashCache
{
    /// <summary>
    /// Resolves a relative path
    /// </summary>
    /// <param name="pathBase">Path base</param>
    /// <param name="contentPath">Content path</param>
    string ResolvePath(PathString pathBase, string contentPath);

    /// <summary>
    /// Resolves a relative path with its hash
    /// </summary>
    /// <param name="pathBase">Path base</param>
    /// <param name="contentUrl">Content url</param>
    /// <returns></returns>
    string ResolveWithHash(PathString pathBase, string contentUrl);

    /// <summary>
    /// Raises scripts changed event causing cache to be cleared
    /// </summary>
    void ScriptsChanged();
}