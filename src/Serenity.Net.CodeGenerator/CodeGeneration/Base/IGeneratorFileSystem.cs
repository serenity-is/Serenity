namespace Serenity
{
    public interface IGeneratorFileSystem
    {
        void Copy(string source, string target, bool overwrite);
        void CreateDirectory(string path);
        void DeleteFile(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string[] GetFiles(string path, string searchPattern, bool recursive = false);
        string[] GetDirectories(string path);
        DateTime GetLastWriteTime(string path);
        string GetFullPath(string path);
        string GetRelativePath(string relativeTo, string path);
        string ReadAllText(string path, Encoding encoding = null);
        byte[] ReadAllBytes(string path);
        void WriteAllText(string path, string content, Encoding encoding = null);
        void WriteAllBytes(string path, byte[] bytes);
    }
}