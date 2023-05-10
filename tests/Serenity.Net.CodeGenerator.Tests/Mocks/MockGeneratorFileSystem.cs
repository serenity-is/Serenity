
namespace Serenity.Tests;

public class MockGeneratorFileSystem : MockFileSystem, IGeneratorFileSystem
{
    public MockGeneratorFileSystem()
    {
    }

    public DateTime GetLastWriteTime(string path)
    {
        return File.GetLastWriteTime(path);
    }
}