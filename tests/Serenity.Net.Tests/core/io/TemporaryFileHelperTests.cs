using Serenity.IO;
using MockFile = System.IO.Abstractions.TestingHelpers.MockFileData;

namespace Serenity.Tests.IO;

public class TemporaryFileHelperTests
{
    [Fact]
    public void DefaultTemporaryCheckFile_Is_Dot_Temporary()
    {
        Assert.Equal(".temporary", TemporaryFileHelper.DefaultTemporaryCheckFile);
    }

    [Fact]
    public void DefaultAutoExpireTime_IsOneDay()
    {
        Assert.Equal(TimeSpan.FromDays(1), TemporaryFileHelper.DefaultAutoExpireTime);
    }

    [Fact]
    public void DefaultMaxFilesInDirectory_Is1000()
    {
        Assert.Equal(1000, TemporaryFileHelper.DefaultMaxFilesInDirectory);
    }

    [Fact]
    public void PurgeDirectoryDefault_ThrowsArgumentNull_IfDirectoryToCleanIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => TemporaryFileHelper.PurgeDirectoryDefault(null, new TemporaryPhysicalFileSystem()));
    }

    [Fact]
    public void PurgeDirectoryDefault_UsesPhysicalFileSystem_IfFileSystemArgumentIsNull()
    {
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName());
        System.IO.File.Delete(tempPath);
        System.IO.Directory.CreateDirectory(tempPath);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(tempPath, ".temporary"), []);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(tempPath, "deleteme"), []);
        System.IO.File.SetCreationTime(System.IO.Path.Combine(tempPath, "deleteme"), DateTime.Now.AddDays(-99999));
        Assert.Equal(2L, System.IO.Directory.GetFiles(tempPath).Length);
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.Zero, maxFilesInDirectory: 0, ".temporary", fileSystem: null);
        var x = Assert.Single(System.IO.Directory.GetFiles(tempPath));
        Assert.Equal(".temporary", System.IO.Path.GetFileName(x));
    }

    [Fact]
    public void PurgeDirectory_UsesPhysicalFileSystem_IfFileSystemArgumentIsNull()
    {
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName());
        System.IO.File.Delete(tempPath);
        System.IO.Directory.CreateDirectory(tempPath);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(tempPath, ".temporary"), []);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(tempPath, "deleteme"), []);
        Assert.Equal(2L, System.IO.Directory.GetFiles(tempPath).Length);
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.Zero, maxFilesInDirectory: 0, ".temporary", fileSystem: null);
        var x = Assert.Single(System.IO.Directory.GetFiles(tempPath));
        Assert.Equal(".temporary", System.IO.Path.GetFileName(x));
    }

    private static MockTemporaryFileSystem CreateTestFileSystem()
    {
        var fileSystem = new MockTemporaryFileSystem();
        var tempPath = fileSystem.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetTempFileName());
        fileSystem.File.Delete(tempPath);
        fileSystem.Directory.CreateDirectory(tempPath);
        fileSystem.Directory.SetCurrentDirectory(tempPath);
        fileSystem.AddFile(".temporary", new MockFile(Array.Empty<byte>()));
        return fileSystem;
    }

    [Fact]
    public void PurgeDirectoryDefault_SkipsPurge_IfNoDotTemporaryFile()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.DeleteFile(".temporary");
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectoryDefault(tempPath, fileSystem);
        Assert.Collection(fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal),
            x => Assert.Equal("file1.txt", x),
            x => Assert.Equal("file2.txt", x),
            x => Assert.Equal("file3.txt", x),
            x => Assert.Equal("file4.txt", x));
    }

    [Fact]
    public void PurgeDirectoryDefault_DeletesFilesOlderThanOneDay()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectoryDefault(tempPath, fileSystem);
        Assert.Collection(fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal),
            x => Assert.Equal(".temporary", x),
            x => Assert.Equal("file3.txt", x),
            x => Assert.Equal("file4.txt", x));
    }

    [Fact]
    public void PurgeDirectoryDefault_DeletesFilesIfMoreThan1000()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        for (var i = 1; i <= 1002; i++)
            fileSystem.AddFile($"file{i:0000}.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-2).AddSeconds(i) });
        TemporaryFileHelper.PurgeDirectoryDefault(tempPath, fileSystem);
        var files = fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(1000, files.Length);
        Assert.Equal(".temporary", files[0]);
        Assert.Equal("file0004.txt", files[1]);
        Assert.Equal("file1002.txt", files[^1]);
    }

    [Fact]
    public void PurgeDirectory_DeletesFiles_IfOlderThanSpecifiedTime()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddMinutes(-65) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddMinutes(-30) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.FromHours(1), 999999, ".temporary", fileSystem);
        Assert.Collection(fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal),
            x => Assert.Equal(".temporary", x),
            x => Assert.Equal("file3.txt", x),
            x => Assert.Equal("file4.txt", x));
    }

    [Fact]
    public void PurgeDirectory_DeletesFiles_IfMoreThanSpecifiedCount()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        for (var i = 1; i <= 12; i++)
            fileSystem.AddFile($"file{i:00}.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-2).AddSeconds(i) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.Zero, 10, ".temporary", fileSystem);
        var files = fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(10, files.Length);
        Assert.Equal(".temporary", files[0]);
        Assert.Equal("file04.txt", files[1]);
        Assert.Equal("file12.txt", files[^1]);
    }

    [Fact]
    public void PurgeDirectoryDefault_SkipsPurge_IfNoSpecifiedCheckFile()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.FromMinutes(5), 1, ".checkme", fileSystem);
        Assert.Collection(fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal),
            x => Assert.Equal(".temporary", x),
            x => Assert.Equal("file1.txt", x),
            x => Assert.Equal("file2.txt", x),
            x => Assert.Equal("file3.txt", x),
            x => Assert.Equal("file4.txt", x));
    }

    [Fact]
    public void PurgeDirectory_DoesNotSkipPurge_IfNullSpecifiedAsCheckFile()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.FromHours(1), 1, checkFileName: null, fileSystem);
        var x = Assert.Single(fileSystem.GetFiles(tempPath).Select(fileSystem.GetFileName));
        Assert.Equal("file4.txt", x);
    }

    [Fact]
    public void PurgeDirectory_DoesNotSkipPurge_IfEmptySpecifiedAsCheckFile()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.FromHours(1), 1, checkFileName: "", fileSystem);
        var x = Assert.Single(fileSystem.GetFiles(tempPath).Select(fileSystem.GetFileName));
        Assert.Equal("file4.txt", x);
    }

    [Fact]
    public void PurgeDirectory_IgnoresError_DuringDelete()
    {
        var fileSystem = CreateTestFileSystem();
        var tempPath = fileSystem.Directory.GetCurrentDirectory();
        fileSystem.AddFile("file1.cantdeletethis", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-2) });
        fileSystem.AddFile("file2.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddDays(-3) });
        fileSystem.AddFile("file3.cantdeletethis", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(-23) });
        fileSystem.AddFile("file4.txt", new MockFile(Array.Empty<byte>()) { CreationTime = DateTime.Now.AddHours(1) });
        TemporaryFileHelper.PurgeDirectory(tempPath, TimeSpan.FromDays(1), 2, checkFileName: ".temporary", fileSystem);
        Assert.Collection(fileSystem.GetFiles(tempPath)
            .Select(fileSystem.GetFileName)
            .OrderBy(x => x, StringComparer.Ordinal),
            x => Assert.Equal(".temporary", x),
            x => Assert.Equal("file1.cantdeletethis", x),
            x => Assert.Equal("file3.cantdeletethis", x),
            x => Assert.Equal("file4.txt", x));
    }
}