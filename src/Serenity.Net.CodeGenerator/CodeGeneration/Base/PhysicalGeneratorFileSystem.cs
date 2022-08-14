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

        public string GetRelativePath(string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path);
        }

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