namespace Serenity.Localization;

public class LocalTextRegistryTests
{
    [Fact]
    public void Add_ThrowsArgumentNull_IfLanguageIDIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().Add(null, "a", "b"));
    }

    [Fact]
    public void Add_ThrowsArgumentNull_IfKeyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().Add("en", null, "b"));
    }

    [Fact]
    public void Add_WorksProperly()
    {
        var registry = new LocalTextRegistry();
        registry.Add("es", "key", "translation");

        var actual = registry.TryGet("es", "key", pending: false);
        Assert.Equal("translation", actual);
    }

    [Fact]
    public void Add_DoesntTrimAny()
    {
        var registry = new LocalTextRegistry();
        registry.Add("  es  ", " key ", " translation ");

        var actual = registry.TryGet("  es  ", " key ", pending: false);
        Assert.Equal(" translation ", actual);
    }

    [Fact]
    public void Add_OverridesExisting()
    {
        var registry = new LocalTextRegistry();
        registry.Add("es", "key", "oldTranslation");

        Assert.Equal("oldTranslation", registry.TryGet("es", "key", pending: false));

        registry.Add("es", "key", "newTranslation");

        Assert.Equal("newTranslation", registry.TryGet("es", "key", pending: false));
    }

    [Fact]
    public void AddPending_ThrowsArgumentNull_IfLanguageIDIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().AddPending(null, "a", "b"));
    }

    [Fact]
    public void AddPending_ThrowsArgumentNull_IfKeyIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().AddPending("en", null, "b"));
    }

    [Fact]
    public void AddPending_WorksProperly()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending("es", "key", "translation");

        Assert.Equal("translation", registry.TryGet("es", "key", pending: true));
        Assert.Null(registry.TryGet("es", "key", pending: false));
    }

    [Fact]
    public void AddPending_DoesntTrimAny()
    {
        var registry = new LocalTextRegistry();
        registry.Add("  es  ", " key ", " translation ");

        var actual = registry.TryGet("  es  ", " key ", pending: true);
        Assert.Equal(" translation ", actual);
    }

    [Fact]
    public void AddPending_OverridesExisting()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending("es", "key", "oldTranslation");

        Assert.Equal("oldTranslation", registry.TryGet("es", "key", pending: true));

        registry.AddPending("es", "key", "newTranslation");

        Assert.Equal("newTranslation", registry.TryGet("es", "key", pending: true));
    }

    [Fact]
    public void AddPending_DoesntOverrideApprovedText()
    {
        var registry = new LocalTextRegistry();
        registry.Add("es", "key", "approvedTranslation");
        Assert.Equal("approvedTranslation", registry.TryGet("es", "key", pending: false));

        registry.AddPending("es", "key", "pendingTranslation1");
        Assert.Equal("pendingTranslation1", registry.TryGet("es", "key", pending: true));
        Assert.Equal("approvedTranslation", registry.TryGet("es", "key", pending: false));

        registry.AddPending("es", "key", "pendingTranslation2");
        Assert.Equal("pendingTranslation2", registry.TryGet("es", "key", pending: true));
        Assert.Equal("approvedTranslation", registry.TryGet("es", "key", pending: false));
    }

    [Fact]
    public void TryGet_Throws_ArgumentNull_If_TextKey_Or_LanguageID_IsNull()
    {
        var registry = new LocalTextRegistry();
        Assert.Throws<ArgumentNullException>(() => registry.TryGet(null, "X", false));
        Assert.Throws<ArgumentNullException>(() => registry.TryGet("X", null, false));
    }

    [Fact]
    public void TryGet_Returns_Pending_Value_IfAvailable()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending("tr", "TestKey", "PendingValue");
        registry.Add("tr", "TestKey", "ApprovedValue");
        Assert.Equal("PendingValue", registry.TryGet("tr", "TestKey", pending: true));
    }

    [Fact]
    public void TryGet_Returns_Null_If_PendingValue_IsNull()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending(LocalText.InvariantLanguageID, "TestKey", null);
        registry.Add(LocalText.InvariantLanguageID, "TestKey", "ApprovedValue");
        Assert.Null(registry.TryGet("tr", "TestKey", pending: true));
    }

    [Fact]
    public void Four_Letter_Language_Fallbacks_Are_Calculated_Automatically()
    {
        var registry = new LocalTextRegistry();
        registry.TryGet("tr-TR", "Test", false);
        registry.TryGet("en-GB", "Test", false);
        registry.TryGet("en-US", "Test", false);

        Assert.Equal(LocalText.InvariantLanguageID, Assert.Single(registry.GetLanguageFallbacks("en")));

        Assert.Collection(registry.GetLanguageFallbacks("en-GB"),
            x => Assert.Equal("en", x),
            x => Assert.Equal(LocalText.InvariantLanguageID, x));

        Assert.Collection(registry.GetLanguageFallbacks("en-US"),
            x => Assert.Equal("en", x),
            x => Assert.Equal(LocalText.InvariantLanguageID, x));

        Assert.Equal(LocalText.InvariantLanguageID, Assert.Single(registry.GetLanguageFallbacks("tr")));

        Assert.Collection(registry.GetLanguageFallbacks("tr-TR"),
            x => Assert.Equal("tr", x),
            x => Assert.Equal(LocalText.InvariantLanguageID, x));
    }

    [Fact]
    public void SetLanguageFallback_ThrowsArgumentNull_IfLanguageIDIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().SetLanguageFallback(null, "en"));
    }

    [Fact]
    public void SetLanguageFallback_ThrowsArgumentNull_IfFallbackIDIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalTextRegistry().SetLanguageFallback("tr", null));
    }

    [Fact]
    public void SetLanguageFallback_WorksProperly()
    {
        var registry = new LocalTextRegistry();
        registry.SetLanguageFallback("tr", "en");
        registry.Add("en", "key", "english");

        Assert.Equal("english", registry.TryGet("tr", "key", false));
    }

    [Fact]
    public void GetAllAvailableTextsInLanguage_ThrowsArgumentNull_IfLanguageIDIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LocalTextRegistry().GetAllAvailableTextsInLanguage(null, false));
    }

    [Fact]
    public void GetAllAvailableTextsInLanguage_ReturnsTexts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key1", "value1");
        registry.Add("en", "key2", "value2");

        var texts = registry.GetAllAvailableTextsInLanguage("en", false);

        Assert.Equal(2, texts.Count);
        Assert.Equal("value1", texts["key1"]);
        Assert.Equal("value2", texts["key2"]);
    }

    [Fact]
    public void GetAllAvailableTextsInLanguage_IncludesFallbackLanguageTexts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key1", "english");
        registry.Add("tr", "key2", "turkish");

        var texts = registry.GetAllAvailableTextsInLanguage("tr", false);

        Assert.Equal("turkish", texts["key2"]);
    }

    [Fact]
    public void GetAllTexts_ReturnsApprovedTexts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key", "value");

        var texts = registry.GetAllTexts(false);
        Assert.Single(texts);
    }

    [Fact]
    public void GetAllTexts_ReturnsPendingTexts()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending("en", "key", "value");

        var texts = registry.GetAllTexts(true);
        Assert.Single(texts);
    }

    [Fact]
    public void GetAllTextKeys_ReturnsKeys()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key1", "value1");
        registry.Add("en", "key2", "value2");

        var keys = registry.GetAllTextKeys(false);
        Assert.Equal(2, keys.Count);
        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
    }

    [Fact]
    public void RemoveAll_ClearsAllTexts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key", "value");
        registry.AddPending("en", "pending", "value");
        registry.SetLanguageFallback("tr", "en");

        registry.RemoveAll();

        Assert.Empty(registry.GetAllTexts(false));
        Assert.Empty(registry.GetAllTexts(true));
        Assert.Empty(registry.GetAllTextKeys(false));
    }

    [Fact]
    public void TryGet_ReturnsNull_ForInvariantLanguage_WhenNotFound()
    {
        var registry = new LocalTextRegistry();
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "missing", false));
    }

    [Fact]
    public void GetLanguageFallbacks_ReturnsEmpty_ForEmptyLanguage()
    {
        var registry = new LocalTextRegistry();
        Assert.Empty(registry.GetLanguageFallbacks(""));
    }

    [Fact]
    public void GetAllAvailableTextsInLanguage_WithPending_IncludesPendingTexts()
    {
        var registry = new LocalTextRegistry();
        registry.AddPending("en", "pending", "value");

        var texts = registry.GetAllAvailableTextsInLanguage("en", pending: true);
        Assert.Equal("value", texts["pending"]);
    }

    [Fact]
    public void GetAllAvailableTextsInLanguage_SkipsDuplicateKeys()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "key", "value1");
        registry.Add("en", "key", "value2");

        var texts = registry.GetAllAvailableTextsInLanguage("en", false);
        Assert.Equal("value2", texts["key"]);
    }
}