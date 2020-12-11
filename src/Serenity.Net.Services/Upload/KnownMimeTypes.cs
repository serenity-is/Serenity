using System;
using System.Collections.Generic;
using System.IO;

namespace Serenity.Web
{
    public static class KnownMimeTypes
    {
        /// <summary>
        ///   (extension -> mime type) pairs for known mime types.</summary>
        private static Dictionary<string, string> knownMimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".bmp", "image/bmp" },
            { ".css", "text/css" },
            { ".gif", "image/gif" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".jpe", "image/jpeg" },
            { ".js", "text/javascript" },
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".swf", "application/x-shockwave-flash" },
            { ".tiff", "image/tiff" },
            { ".txt", "text/plain" }
        };

        /// <summary>
        ///   Gets MIME type for a given file using information in Win32 HKEY_CLASSES_ROOT 
        ///   registry key.</summary>
        /// <param name="path">
        ///   File name whose MIME type will be determined. Its only extension part will be used.</param>
        /// <returns>
        ///   Determined mime type for given file. "application/unknown" otherwise.</returns>
        public static string Get(string path)
        {
            return TryGet(path) ?? "application/unknown";
        }

        /// <summary>
        ///   Gets MIME type for a given file using information in Win32 HKEY_CLASSES_ROOT 
        ///   registry key.</summary>
        /// <param name="path">
        ///   File name whose MIME type will be determined. Its only extension part will be used.</param>
        /// <returns>
        ///   Determined mime type for given file. "application/unknown" otherwise.</returns>
        public static string TryGet(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string ext = Path.GetExtension(path);
            return knownMimeTypes.TryGetValue(ext, out string mimeType) ? mimeType : null;
        }

    }
}