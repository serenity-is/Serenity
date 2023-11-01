namespace Serenity.Tests;

public class MockTemporaryFileSystem : MockFileSystem, ITemporaryFileSystem
{
    public MockTemporaryFileSystem(string currentDirectory = "")
        : base(currentDirectory)
    {
    }

    public DateTime GetLastWriteTimeUtc(string path)
    {
        return File.GetLastWriteTimeUtc(path);
    }

    public TemporaryFileInfo[] GetTemporaryFileInfos(string path)
    {
        return DirectoryInfo.New(path).GetFiles().Select(x => new TemporaryFileInfo
        {
            CreationTime = x.CreationTime,
            FullName = x.FullName,
            Name = x.Name
        }).ToArray();
    }

    public new void DeleteFile(string filename)
    {
        if (Path.GetExtension(filename) == ".cantdeletethis")
            throw new System.IO.IOException("Can't delete these files in tests!");

        base.DeleteFile(filename);
    }
}