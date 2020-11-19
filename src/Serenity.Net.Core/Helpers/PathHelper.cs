using System;
using System.IO;
using System.Linq;

namespace Serenity
{
    /// <summary>
    /// Contains Path related helper functions.
    /// </summary>
    public static class PathHelper
    {
        static readonly char[] invalidChars = Path.GetInvalidFileNameChars()
            .Where(x => x != '/' && x != '\\').ToArray();

        /// <summary>
        /// Checks whether given path is a secure relative path
        /// </summary>
        /// <param name="relativePath">Relative path</param>
        /// <returns>True if relative path looks safe</returns>
        public static bool IsSecureRelativePath(string relativePath)
        {
            var trim = relativePath.TrimToNull();

            return !(trim == null ||
                trim == "." ||
                trim == ".." ||
                relativePath.IndexOf("../") >= 0 ||
                relativePath.IndexOf("..\\") >= 0 ||
                relativePath.IndexOf(':') >= 0 ||
                trim.StartsWith("/") ||
                trim.StartsWith("\\") ||
                Path.IsPathRooted(relativePath) ||
                relativePath.IndexOfAny(invalidChars) >= 0 ||
                !Path.Combine("a/", relativePath).StartsWith("a/"));
        }

        /// <summary>
        /// Combines a path and filename checking security
        /// </summary>
        /// <param name="root">Root path</param>
        /// <param name="relativePath">Relative path. Should not be rooted, not containing .. etc.</param>
        /// <returns>Combined path</returns>
        public static string SecureCombine(string root, string relativePath)
        {
            if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException(nameof(root));

            if (string.IsNullOrEmpty(relativePath))
                throw new ArgumentNullException(nameof(relativePath));

            if (!IsSecureRelativePath(relativePath))
                throw new ArgumentOutOfRangeException(nameof(relativePath));

            return Path.Combine(root, relativePath);
        }

        /// <summary>
        /// Checks whether given path is a secure relative file
        /// </summary>
        /// <param name="relativeFile">Relative file</param>
        /// <returns>True if relative file looks safe</returns>
        public static bool IsSecureRelativeFile(string relativeFile)
        {
            if (!IsSecureRelativePath(relativeFile))
                return false;

            var trimRight = relativeFile.TrimEnd();
            if (trimRight.EndsWith('/') || trimRight.EndsWith('\\'))
                return false;

            return true;
        }
    }
}