using System.IO;

namespace Serenity;

public class PhysicalFileSystemTests : IDisposable
{
    private readonly string tempDir;
    private readonly PhysicalFileSystem fs;

    public PhysicalFileSystemTests()
    {
        tempDir = Path.Combine(Path.GetTempPath(), "Serenity_PhysicalFileSystem_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        fs = new PhysicalFileSystem();
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, recursive: true);
    }

    private string PathOf(string name) => Path.Combine(tempDir, name);

    [Fact]
    public void CreateDirectory_CreatesDirectory()
    {
        var path = PathOf("sub");
        fs.CreateDirectory(path);
        Assert.True(Directory.Exists(path));
    }

    [Fact]
    public void CreateFile_CreatesFile()
    {
        var path = PathOf("file.txt");
        using var stream = fs.CreateFile(path);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void CreateFile_WithoutOverwrite_Throws_WhenFileExists()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "content");
        Assert.Throws<IOException>(() => fs.CreateFile(path, overwrite: false));
    }

    [Fact]
    public void DeleteDirectory_DeletesDirectory()
    {
        var path = PathOf("sub");
        fs.CreateDirectory(path);
        fs.DeleteDirectory(path, recursive: true);
        Assert.False(Directory.Exists(path));
    }

    [Fact]
    public void DeleteFile_DeletesFile()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "content");
        fs.DeleteFile(path);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void DirectoryExists_ReturnsTrue_WhenExists()
    {
        var path = PathOf("sub");
        fs.CreateDirectory(path);
        Assert.True(fs.DirectoryExists(path));
        Assert.False(fs.DirectoryExists(PathOf("missing")));
    }

    [Fact]
    public void FileExists_ReturnsTrue_WhenExists()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "content");
        Assert.True(fs.FileExists(path));
        Assert.False(fs.FileExists(PathOf("missing.txt")));
    }

    [Fact]
    public void GetDirectories_ReturnsSubdirectories()
    {
        fs.CreateDirectory(PathOf("a"));
        fs.CreateDirectory(PathOf("b"));
        var dirs = fs.GetDirectories(tempDir);
        Assert.Equal(2, dirs.Length);
    }

    [Fact]
    public void GetFiles_ReturnsFiles()
    {
        fs.WriteAllText(PathOf("a.txt"), "a");
        fs.WriteAllText(PathOf("b.txt"), "b");
        var files = fs.GetFiles(tempDir);
        Assert.Equal(2, files.Length);
    }

    [Fact]
    public void GetFileSize_ReturnsSize()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "12345");
        Assert.Equal(5, fs.GetFileSize(path));
    }

    [Fact]
    public void GetFullPath_ReturnsFullPath()
    {
        Assert.Equal(Path.GetFullPath(tempDir), fs.GetFullPath(tempDir));
    }

    [Fact]
    public void GetLastWriteTimeUtc_ReturnsTime()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "content");
        Assert.True(fs.GetLastWriteTimeUtc(path) > DateTime.MinValue);
    }

    [Fact]
    public void GetRelativePath_ReturnsRelativePath()
    {
        var relative = fs.GetRelativePath(tempDir, PathOf("file.txt"));
        Assert.Equal("file.txt", relative);
    }

    [Fact]
    public void OpenRead_ReturnsStream()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "content");
        using var stream = fs.OpenRead(path);
        Assert.True(stream.CanRead);
    }

    [Fact]
    public void ReadAllBytes_ReturnsBytes()
    {
        var path = PathOf("file.txt");
        fs.WriteAllBytes(path, [1, 2, 3]);
        Assert.Equal(new byte[] { 1, 2, 3 }, fs.ReadAllBytes(path));
    }

    [Fact]
    public void ReadAllText_ReturnsText()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "hello");
        Assert.Equal("hello", fs.ReadAllText(path));
    }

    [Fact]
    public void ReadAllText_WithEncoding_ReturnsText()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "hello", Encoding.UTF8);
        Assert.Equal("hello", fs.ReadAllText(path, Encoding.UTF8));
    }

    [Fact]
    public void WriteAllBytes_WritesBytes()
    {
        var path = PathOf("file.txt");
        fs.WriteAllBytes(path, [1, 2, 3]);
        Assert.Equal(new byte[] { 1, 2, 3 }, File.ReadAllBytes(path));
    }

    [Fact]
    public void WriteAllText_WritesText()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "hello");
        Assert.Equal("hello", File.ReadAllText(path));
    }

    [Fact]
    public void WriteAllText_WithEncoding_WritesText()
    {
        var path = PathOf("file.txt");
        fs.WriteAllText(path, "hello", Encoding.UTF8);
        Assert.Equal("hello", File.ReadAllText(path, Encoding.UTF8));
    }
}