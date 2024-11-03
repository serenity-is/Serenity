using Serenity.Localization;

namespace Serenity.Tests.Localization;

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
        Assert.Collection(registry.GetLanguageFallbacks().OrderBy(x => x.Key),
            x =>
            {
                Assert.Equal("en", x.Key);
                Assert.Equal("", x.Value);
            }, 
            x =>
            {
                Assert.Equal("en-GB", x.Key);
                Assert.Equal("en", x.Value);
            },
            x =>
            {
                Assert.Equal("en-US", x.Key);
                Assert.Equal("en", x.Value);
            },
            x =>
            {
                Assert.Equal("tr", x.Key);
                Assert.Equal("", x.Value);
            },
            x =>
            {
                Assert.Equal("tr-TR", x.Key);
                Assert.Equal("tr", x.Value);
            });
    }
}