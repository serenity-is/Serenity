using System.IO;

namespace Serenity.CodeGeneration
{
    public class PhysicalGeneratorFileSystem : IGeneratorFileSystem
    {
        public void Copy(string source, string target, bool overwrite)
        {
            File.Copy(source, target, overwrite);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string searchPattern, bool recursive = false)
        {
            return Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly);
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

#if ISSOURCEGENERATOR
        // https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/32113484#32113484
        public string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");

            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            fromPath = fromPath.Replace("/", "\\", StringComparison.Ordinal);
            toPath = toPath.Replace("/", "\\", StringComparison.Ordinal);

            if (!fromPath.Contains(':', StringComparison.Ordinal))
                fromPath = "z:\\" + fromPath;

            if (!toPath.Contains(':', StringComparison.Ordinal))
                toPath = "z:\\" + toPath;

            Uri fromUri = new(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
                return toPath;

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;

            return path;
        }
#else
        public string GetRelativePath(string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path);
        }
#endif

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public string ReadAllText(string path, Encoding encoding = null)
        {
            return encoding != null ?
                File.ReadAllText(path, encoding) :
                File.ReadAllText(path);
        }

        public void RemoveReadOnly(string path)
        {
            var attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.ReadOnly))
            {
                attr -= FileAttributes.ReadOnly;
                File.SetAttributes(path, attr);
            }
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public void WriteAllText(string path, string content, Encoding encoding = null)
        {
            if (encoding != null)
            {
                File.WriteAllText(path, content, encoding);
                return;
            }

            File.WriteAllText(path, content);
        }
    }
}