using System.IO;
using System.IO.Abstractions;

namespace Serenity.CodeGeneration
{
    public class AbstractedFileSystem : IGeneratorFileSystem
    {
        private readonly IFileSystem fileSystem;

        public AbstractedFileSystem(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return fileSystem.Directory.GetFiles(path, searchPattern, searchOption);
        }

        public string GetFullPath(string path)
        {
            return fileSystem.Path.GetFullPath(path);
        }

        public string ReadAllText(string path)
        {
            return fileSystem.File.ReadAllText(path);
        }
    }
}