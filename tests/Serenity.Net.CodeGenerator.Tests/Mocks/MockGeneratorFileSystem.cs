
namespace Serenity.Tests.CodeGenerator
{
    public class MockGeneratorFileSystem : IGeneratorFileSystem
    {
        private readonly IFileSystem fileSystem;

        public MockGeneratorFileSystem(IFileSystem fileSystem = null)
        {
            this.fileSystem = fileSystem ?? new MockFileSystem();
        }

        public void Copy(string source, string target, bool overwrite)
        {
            fileSystem.File.Copy(source, target, overwrite);
        }

        public void CreateDirectory(string path)
        {
            fileSystem.Directory.CreateDirectory(path);
        }

        public void DeleteFile(string path)
        {
            fileSystem.File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return fileSystem.Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return fileSystem.File.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return fileSystem.Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string searchPattern, bool recursive = false)
        {
            return fileSystem.Directory.GetFiles(path, searchPattern, 
                recursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
        }

        public string GetFullPath(string path)
        {
            return fileSystem.Path.GetFullPath(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return fileSystem.File.GetLastWriteTime(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return fileSystem.File.ReadAllBytes(path);
        }

        public string ReadAllText(string path, Encoding encoding = null)
        {
            return encoding != null ?
                fileSystem.File.ReadAllText(path, encoding) :
                fileSystem.File.ReadAllText(path);
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            fileSystem.File.WriteAllBytes(path, bytes);
        }

        public void WriteAllText(string path, string content, Encoding encoding = null)
        {
            if (encoding != null)
            {
                fileSystem.File.WriteAllText(path, content, encoding);
                return;
            }

            fileSystem.File.WriteAllText(path, content);
        }
    }
}