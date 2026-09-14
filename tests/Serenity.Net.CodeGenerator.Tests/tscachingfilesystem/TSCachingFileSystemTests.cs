using System.IO;

namespace Serenity.CodeGenerator;

public class TSCachingFileSystemTests
{
    [Fact]
    public void Constructor_Throws_ForNullFileSystem()
    {
        Assert.Throws<ArgumentNullException>(() => new TSCachingFileSystem(null!));
    }

    [Fact]
    public void FileExists_ReturnsFalse_ForNullOrEmptyPath()
    {
        var fileSystem = new TSCachingFileSystem(new MockFileSystem());

        Assert.False(fileSystem.FileExists(null!));
        Assert.False(fileSystem.FileExists(""));
    }

    [Fact]
    public void FileExists_CachesResult()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new(""));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.True(fileSystem.FileExists("/root/a.ts"));

        inner.DeleteFile("/root/a.ts");

        Assert.True(fileSystem.FileExists("/root/a.ts"));
    }

    [Fact]
    public void DirectoryExists_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new(""));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.True(fileSystem.DirectoryExists("/root"));
        Assert.False(fileSystem.DirectoryExists("/missing"));
    }

    [Fact]
    public void ReadAllText_Throws_ForNullOrEmptyPath()
    {
        var fileSystem = new TSCachingFileSystem(new MockFileSystem());

        Assert.Throws<ArgumentNullException>(() => fileSystem.ReadAllText(null!));
        Assert.Throws<ArgumentNullException>(() => fileSystem.ReadAllText(""));
    }

    [Fact]
    public void ReadAllText_Throws_ForNonUtf8Encoding()
    {
        var fileSystem = new TSCachingFileSystem(new MockFileSystem());

        Assert.Throws<NotImplementedException>(() =>
            fileSystem.ReadAllText("/root/a.ts", Encoding.ASCII));
    }

    [Fact]
    public void ReadAllText_AllowsUtf8Encoding()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new("hello"));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal("hello", fileSystem.ReadAllText("/root/a.ts", Encoding.UTF8));
    }

    [Fact]
    public void ReadAllText_CachesResult()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new("first"));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal("first", fileSystem.ReadAllText("/root/a.ts"));

        inner.WriteAllText("/root/a.ts", "second");

        Assert.Equal("first", fileSystem.ReadAllText("/root/a.ts"));
    }

    [Fact]
    public void ReadAllText_UsesNormalizedCacheKey()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new("hello"));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal("hello", fileSystem.ReadAllText("/root/a.ts"));

        inner.WriteAllText("/root/a.ts", "changed");

        Assert.Equal("hello", fileSystem.ReadAllText("/root\\a.ts"));
    }

    [Fact]
    public void GetDirectories_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/sub/a.ts", new(""));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Single(fileSystem.GetDirectories("/root"));
    }

    [Fact]
    public void GetFiles_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new(""));
        inner.AddFile("/root/b.ts", new(""));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal(2, fileSystem.GetFiles("/root").Length);
        Assert.Single(fileSystem.GetFiles("/root", "a.*"));
    }

    [Fact]
    public void GetFileSize_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new("12345"));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal(5, fileSystem.GetFileSize("/root/a.ts"));
    }

    [Fact]
    public void GetFullPath_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal(inner.GetFullPath("/root/a.ts"), fileSystem.GetFullPath("/root/a.ts"));
    }

    [Fact]
    public void GetLastWriteTimeUtc_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        inner.AddFile("/root/a.ts", new("hello"));
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.True(fileSystem.GetLastWriteTimeUtc("/root/a.ts") > DateTime.MinValue);
    }

    [Fact]
    public void GetRelativePath_DelegatesToInnerFileSystem()
    {
        var inner = new MockFileSystem();
        var fileSystem = new TSCachingFileSystem(inner);

        Assert.Equal("a.ts", fileSystem.GetRelativePath("/root", "/root/a.ts"));
    }

    [Fact]
    public void UnsupportedOperations_ThrowNotImplemented()
    {
        var fileSystem = new TSCachingFileSystem(new MockFileSystem());

        Assert.Throws<NotImplementedException>(() => fileSystem.CreateDirectory("/root"));
        Assert.Throws<NotImplementedException>(() => fileSystem.CreateFile("/root/a.ts"));
        Assert.Throws<NotImplementedException>(() => fileSystem.DeleteDirectory("/root", true));
        Assert.Throws<NotImplementedException>(() => fileSystem.DeleteFile("/root/a.ts"));
        Assert.Throws<NotImplementedException>(() => fileSystem.OpenRead("/root/a.ts"));
        Assert.Throws<NotImplementedException>(() => fileSystem.ReadAllBytes("/root/a.ts"));
        Assert.Throws<NotImplementedException>(() => fileSystem.WriteAllBytes("/root/a.ts", []));
        Assert.Throws<NotImplementedException>(() => fileSystem.WriteAllText("/root/a.ts", "x"));
    }
}
