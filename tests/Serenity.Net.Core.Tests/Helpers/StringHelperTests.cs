namespace Serenity.Tests.Helpers;

public class StringHelperTests
{
    [Fact]
    public void IsEmptyOrNull_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsEmptyOrNull());
    }

    [Fact]
    public void IsEmptyOrNull_WhitespaceString_ReturnsFalse()
    {
        Assert.False("   ".IsEmptyOrNull());
    }

    [Fact]
    public void IsNullOrEmpty_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_WhitespaceString_ReturnsFalse()
    {
        Assert.False("   ".IsNullOrEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_NullString_ReturnsTrue()
    {
        Assert.True(((string)null).IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_EmptyString_ReturnsTrue()
    {
        Assert.True(string.Empty.IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_NonEmptyString_ReturnsFalse()
    {
        Assert.False("Hello, world!".IsTrimmedEmpty());
    }

    [Fact]
    public void IsTrimmedEmpty_WhitespaceString_ReturnsTrue()
    {
        Assert.True("   ".IsTrimmedEmpty());
    }
}