using System.IO;

namespace Serenity;

public class PathHelperTests
{
    [Theory]
    [InlineData("file.txt", true)]
    [InlineData("folder/file.txt", true)]
    [InlineData("folder\\file.txt", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(".", false)]
    [InlineData("..", false)]
    [InlineData("../file.txt", false)]
    [InlineData("..\\file.txt", false)]
    [InlineData("C:/file.txt", false)]
    [InlineData("/file.txt", false)]
    [InlineData("\\file.txt", false)]
    public void IsSecureRelativePath_ReturnsExpected(string? path, bool expected)
    {
        Assert.Equal(expected, PathHelper.IsSecureRelativePath(path));
    }

    [Fact]
    public void SecureCombine_ThrowsArgumentNullException_WhenRootIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => PathHelper.SecureCombine(null, "file.txt"));
    }

    [Fact]
    public void SecureCombine_ThrowsArgumentNullException_WhenRelativePathIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => PathHelper.SecureCombine("C:/", null));
    }

    [Fact]
    public void SecureCombine_ThrowsArgumentOutOfRangeException_ForUnsafePath()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PathHelper.SecureCombine("C:/", "../file.txt"));
    }

    [Fact]
    public void SecureCombine_CombinesPaths()
    {
        Assert.Equal(Path.Combine("C:/root", "file.txt"), PathHelper.SecureCombine("C:/root", "file.txt"));
    }

    [Fact]
    public void IsSecureRelativeFile_ReturnsFalse_ForDirectoryPath()
    {
        Assert.False(PathHelper.IsSecureRelativeFile("folder/"));
        Assert.False(PathHelper.IsSecureRelativeFile("folder\\"));
    }

    [Fact]
    public void IsSecureRelativeFile_ReturnsTrue_ForFile()
    {
        Assert.True(PathHelper.IsSecureRelativeFile("file.txt"));
    }

    [Fact]
    public void ValidateSecureRelativeFile_Throws_ForUnsafeFile()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PathHelper.ValidateSecureRelativeFile("../file.txt"));
    }

    [Fact]
    public void ValidateSecureRelativeFile_DoesNotThrow_ForSafeFile()
    {
        PathHelper.ValidateSecureRelativeFile("file.txt");
    }

    [Fact]
    public void ToUrl_ConvertsBackslashes()
    {
        Assert.Equal("folder/file.txt", PathHelper.ToUrl("folder\\file.txt"));
        Assert.Equal("file.txt", PathHelper.ToUrl("file.txt"));
        Assert.Null(PathHelper.ToUrl(null));
    }

    [Fact]
    public void ToPath_ConvertsForwardSlashes()
    {
        var separator = Path.DirectorySeparatorChar;
        Assert.Equal("folder" + separator + "file.txt", PathHelper.ToPath("folder/file.txt"));
        Assert.Equal("file.txt", PathHelper.ToPath("file.txt"));
        Assert.Null(PathHelper.ToPath(null));
    }
}