using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Contains a set of known popular mime types.
/// </summary>
public static class KnownMimeTypes
{
    private static readonly FileExtensionContentTypeProvider contentTypeProvider = new();

    /// <summary>
    ///   (extension -> mime type) pairs for known mime types.</summary>
    private static readonly Dictionary<string, string> knownMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".apng", "image/apng" },
        { ".avif", "image/avif" }
    };

    /// <summary>
    ///   Gets the MIME type for a given file using information in the Win32 HKEY_CLASSES_ROOT
    ///   registry key.</summary>
    /// <param name="path">
    ///   The file name whose MIME type will be determined. Only its extension part will be used.</param>
    /// <returns>
    ///   The determined mime type for the given file, or <c>application/octet-stream</c> otherwise.</returns>
    public static string Get(string path)
    {
        return TryGet(path) ?? "application/octet-stream";
    }

    /// <summary>
    ///   Gets the MIME type for a given file.</summary>
    /// <param name="path">
    ///   The file name whose MIME type will be determined. Only its extension part will be used.</param>
    /// <returns>
    ///   The determined mime type for the given file, or <c>null</c> if unknown.</returns>
    public static string TryGet(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        string ext = Path.GetExtension(path);
        if (knownMimeTypes.TryGetValue(ext, out string mimeType))
            return mimeType;

        if (!contentTypeProvider.TryGetContentType(path, out mimeType))
            return null;
        
        return mimeType;
    }
}