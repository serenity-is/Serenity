namespace Serenity.Localization;

public class FallbackLocalTextSourceTests
{
    private static MockLocalTextRegistry NewRegistry()
    {
        return new MockLocalTextRegistry();
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FallbackLocalTextSource(null));
    }

    [Fact]
    public void TryGet_ReturnsText_WhenFound()
    {
        var source = NewRegistry();
        source.Add("en", "key", "value");
        var fallback = new FallbackLocalTextSource(source);

        Assert.Equal("value", fallback.TryGet("en", "key", false));
    }

    [Fact]
    public void TryGet_ReturnsText_WhenKeyIsEmpty()
    {
        var source = NewRegistry();
        var fallback = new FallbackLocalTextSource(source);

        Assert.Null(fallback.TryGet("en", "", false));
    }

    [Fact]
    public void TryGet_ReturnsNull_WhenNotFound()
    {
        var source = NewRegistry();
        var fallback = new FallbackLocalTextSource(source);

        Assert.Null(fallback.TryGet("en", "missing", false));
    }

    [Fact]
    public void TryGet_RemovesEntitySingularSuffix_AndFindsBase()
    {
        var source = NewRegistry();
        source.Add("en", "Some.Entity", "Entity Text");
        var fallback = new FallbackLocalTextSource(source);

        Assert.Equal("Entity Text", fallback.TryGet("en", "Some.Entity.EntitySingular", false));
    }

    [Fact]
    public void TryGet_RemovesEntityPluralSuffix_AndFindsBase()
    {
        var source = NewRegistry();
        source.Add("en", "Some.Entity", "Entity Text");
        var fallback = new FallbackLocalTextSource(source);

        Assert.Equal("Entity Text", fallback.TryGet("en", "Some.Entity.EntityPlural", false));
    }

    [Fact]
    public void TryGet_FallsBackToSubKey()
    {
        var source = NewRegistry();
        source.Add("en", "June", "June Text");
        var fallback = new FallbackLocalTextSource(source);

        Assert.Equal("June Text", fallback.TryGet("en", "Enums.Month.June", false));
    }

    [Fact]
    public void TryGet_BreaksUpString_WhenSubKeyNotFound()
    {
        var source = NewRegistry();
        var fallback = new FallbackLocalTextSource(source);

        Assert.Equal("Last Directory Update", fallback.TryGet("en", "Some.LastDirectoryUpdate", false));
    }

    [Fact]
    public void TryGetKeyFallback_ReturnsLastPartAfterDot()
    {
        Assert.Equal("June", FallbackLocalTextSource.TryGetKeyFallback("Enums.Month.June"));
    }

    [Fact]
    public void TryGetKeyFallback_RemovesIdSuffix()
    {
        Assert.Equal("User", FallbackLocalTextSource.TryGetKeyFallback("Some.UserId"));
    }

    [Fact]
    public void TryGetKeyFallback_ReturnsLastPartAfterSlash()
    {
        Assert.Equal("Update", FallbackLocalTextSource.TryGetKeyFallback("Some/Update"));
    }

    [Fact]
    public void TryGetKeyFallback_ReturnsNull_WhenNoSeparator()
    {
        Assert.Null(FallbackLocalTextSource.TryGetKeyFallback("NoSeparator"));
    }

    [Fact]
    public void BreakUpString_InsertsSpacesBeforeCapitals()
    {
        Assert.Equal("Last Directory Update", FallbackLocalTextSource.BreakUpString("LastDirectoryUpdate"));
    }

    [Fact]
    public void Add_DelegatesToSource()
    {
        var source = NewRegistry();
        var fallback = new FallbackLocalTextSource(source);

        fallback.Add("en", "key", "value");
        Assert.Equal("value", source.TryGet("en", "key", false));
    }
}
