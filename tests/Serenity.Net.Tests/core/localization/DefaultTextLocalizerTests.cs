using System.Globalization;

namespace Serenity.Localization;

public class DefaultTextLocalizerTests
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenRegistryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultTextLocalizer(null));
    }

    [Fact]
    public void TryGet_ReturnsTranslation_FromRegistry()
    {
        var registry = new MockLocalTextRegistry();
        registry.Add(CultureInfo.CurrentUICulture.Name, "key", "value");
        var localizer = new DefaultTextLocalizer(registry);

        Assert.Equal("value", localizer.TryGet("key"));
    }

    [Fact]
    public void TryGet_ReturnsNull_WhenNotFound()
    {
        var registry = new MockLocalTextRegistry();
        var localizer = new DefaultTextLocalizer(registry);

        Assert.Null(localizer.TryGet("missing"));
    }
}
