using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Contains set of known popular mime types
/// </summary>
public static class KnownMimeTypes
{
    private static readonly IContentTypeProvider contentTypeProvider = new FileExtensionContentTypeProvider();

    /// <summary>
    ///   (extension -> mime type) pairs for known mime types.</summary>
    private static readonly Dictionary<string, string> knownMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".apng", "image/apng" },
        { ".avif", "image/avif" }
    };

    /// <summary>
    ///   Gets MIME type for a given file using information in Win32 HKEY_CLASSES_ROOT 
    ///   registry key.</summary>
    /// <param name="path">
    ///   File name whose MIME type will be determined. Its only extension part will be used.</param>
    /// <returns>
    ///   Determined mime type for given file. "application/octet-stream" otherwise.</returns>
    public static string Get(string path)
    {
        return TryGet(path) ?? "application/octet-stream";
    }

    /// <summary>
    ///   Gets MIME type for a given file</summary>
    /// <param name="path">
    ///   File name whose MIME type will be determined. Its only extension part will be used.</param>
    /// <returns>
    ///   Determined mime type for given file. "application/octet-stream" otherwise.</returns>
    public static string TryGet(string path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        string ext = Path.GetExtension(path);
        if (knownMimeTypes.TryGetValue(ext, out string mimeType))
            return mimeType;

        if (!contentTypeProvider.TryGetContentType(path, out mimeType))
            return null;
        
        return mimeType;
    }
}