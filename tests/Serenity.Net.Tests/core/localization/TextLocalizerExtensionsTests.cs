namespace Serenity;

public class TextLocalizerExtensionsTests
{
    [Fact]
    public void Get_ThrowsArgumentNullException_WhenLocalizerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ((ITextLocalizer)null).Get("key"));
    }

    [Fact]
    public void Get_ReturnsTranslation_WhenFound()
    {
        var localizer = new MockTextLocalizer(key => "translated");
        Assert.Equal("translated", localizer.Get("key"));
    }

    [Fact]
    public void Get_ReturnsKey_WhenNotFound()
    {
        var localizer = new MockTextLocalizer(key => null);
        Assert.Equal("key", localizer.Get("key"));
    }
}
