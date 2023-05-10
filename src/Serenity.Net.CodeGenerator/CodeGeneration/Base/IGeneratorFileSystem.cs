namespace Serenity;

public interface IGeneratorFileSystem : IFileSystem
{
    DateTime GetLastWriteTime(string path);
}