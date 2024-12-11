namespace Serenity.ComponentModel;

public class DefaultFilenameFormatSanitizerTests
{
    [Fact]
    public void Instance_ReturnsNonNullSingleton()
    {
        var instance = DefaultFilenameFormatSanitizer.Instance;
        Assert.NotNull(instance);
        Assert.IsType<DefaultFilenameFormatSanitizer>(instance);
    }

    [Fact]
    public void SanitizePlaceholder_ReplacesBackslashesAndDiacritics()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", @"filen\ame..txt");
        Assert.Equal("filen_ame_txt", result);
    }

    [Fact]
    public void SanitizePlaceholder_ReplacesMultipleDots()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", "file..name");
        Assert.Equal("file_name", result);
    }

    [Fact]
    public void SanitizePlaceholder_HandlesEmptyOrWhitespace()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", "  ");
        Assert.Equal("_", result);
    }

    [Fact]
    public void SanitizePlaceholder_TrimsTrailingDots()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", "filename.");
        Assert.Equal("filename_", result);
    }

    [Fact]
    public void SanitizePlaceholder_ReturnsUnderscore_ForNullValue()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", null);
        Assert.Equal("_", result);
    }

    [Fact]
    public void SanitizePlaceholder_ReturnsUnderscore_ForEmptyValue()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", "");
        Assert.Equal("_", result);
    }

    [Fact]
    public void SanitizePlaceholder_ReturnsUnderscore_ForWhitespaceValue()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizePlaceholder("", "   ");
        Assert.Equal("_", result);
    }

    [Fact]
    public void SanitizeResult_ReturnsUnderscoreAsIs()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult("_");
        Assert.Equal("_", result);
    }

    [Fact]
    public void SanitizeResult_ReturnsEmptyStringAsIs()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult("");
        Assert.Equal("", result);
    }

    [Fact]
    public void SanitizeResult_ReturnsNullAsIs()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult(null);
        Assert.Null(result);
    }

    [Fact]
    public void SanitizeResult_ReplacesBackslashes_WithSlashes()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult("path\\to\\file");
        Assert.Equal("path/to/file", result);
    }

    [Fact]
    public void SanitizeResult_ReplacesDoubleSlashes_WithSlashUnderscoreSlash()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult("path//to//file");
        Assert.Equal("path/_/to/_/file", result);
    }

    [Fact]
    public void SanitizeResult_HandlesComplexPath()
    {
        var attribute = new DefaultFilenameFormatSanitizer();
        string result = attribute.SanitizeResult("path\\to//some\\file");
        Assert.Equal("path/to/_/some/file", result);
    }
}