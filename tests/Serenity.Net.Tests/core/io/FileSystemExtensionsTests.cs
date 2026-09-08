using System.IO;

namespace Serenity;

public class FileSystemExtensionsTests
{
    private static readonly IFileSystem fs = new PhysicalFileSystem();

    [Fact]
    public void ChangeExtension_ChangesExtension()
    {
        Assert.Equal("a.txt", fs.ChangeExtension("a.bin", ".txt"));
    }

    [Fact]
    public void Combine_TwoPaths_Combines()
    {
        Assert.Equal(Path.Combine("a", "b"), fs.Combine("a", "b"));
    }

    [Fact]
    public void Combine_ThreePaths_Combines()
    {
        Assert.Equal(Path.Combine("a", "b", "c"), fs.Combine("a", "b", "c"));
    }

    [Fact]
    public void Combine_Array_Combines()
    {
        Assert.Equal(Path.Combine("a", "b", "c"), fs.Combine("a", "b", "c"));
    }

    [Fact]
    public void Combine_ParamsArray_Combines()
    {
        Assert.Equal(Path.Combine("a", "b", "c", "d"), fs.Combine("a", "b", "c", "d"));
    }

    [Fact]
    public void Copy_CopiesFile()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "Serenity_FsExt_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var source = Path.Combine(tempDir, "source.txt");
            var dest = Path.Combine(tempDir, "dest.txt");
            File.WriteAllText(source, "content");
            fs.Copy(source, dest, overwrite: true);
            Assert.Equal("content", File.ReadAllText(dest));
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public void GetDirectoryName_ReturnsDirectory()
    {
        Assert.Equal(Path.GetDirectoryName("a/b.txt"), fs.GetDirectoryName("a/b.txt"));
    }

    [Fact]
    public void GetFileName_ReturnsFileName()
    {
        Assert.Equal("b.txt", fs.GetFileName("a/b.txt"));
    }

    [Fact]
    public void GetFileNameWithoutExtension_ReturnsName()
    {
        Assert.Equal("b", fs.GetFileNameWithoutExtension("a/b.txt"));
    }

    [Fact]
    public void GetExtension_ReturnsExtension()
    {
        Assert.Equal(".txt", fs.GetExtension("a/b.txt"));
    }
}