namespace Serenity.CodeGeneration
{
    public interface IGeneratorFileSystem
    {
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption);
        string[] GetDirectories(string path);
        string GetFullPath(string path);
        string ReadAllText(string path);
    }
}