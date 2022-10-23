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
    public void LocalTextRegistry_Add_DoesntTrimAny()
    {
        var registry = new LocalTextRegistry();
        registry.Add("  es  ", " key ", " translation ");

        var actual = registry.TryGet("  es  ", " key ", pending: false);
        Assert.Equal(" translation ", actual);
    }

    [Fact]
    public void LocalTextRegistry_Add_OverridesExisting()
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
    public void LocalTextRegistry_AddPending_DoesntOverrideApprovedText()
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
}