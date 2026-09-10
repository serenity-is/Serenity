namespace Serenity.Data;

public class DefaultRowTypeRegistryTests
{
    private abstract class AbstractRow : IdNameRow
    {
    }

    private class NotARow
    {
    }

    private interface INonRow
    {
    }

    [ConnectionKey("TestKey")]
    private class KeyedRow : IdNameRow
    {
    }

    [ConnectionKey("OtherKey")]
    private class OtherKeyedRow : IdNameRow
    {
    }

    [Fact]
    public void Constructor_NullTypeSource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultRowTypeRegistry(null!));
    }

    [Fact]
    public void AllRowTypes_ReturnsOnlyConcreteRowTypes()
    {
        var typeSource = new MockTypeSource(
            typeof(IdNameRow),
            typeof(AbstractRow),
            typeof(NotARow),
            typeof(INonRow));
        var registry = new DefaultRowTypeRegistry(typeSource);

        var rowTypes = registry.AllRowTypes.ToList();

        Assert.Equal(new[] { typeof(IdNameRow) }, rowTypes);
    }

    [Fact]
    public void ByConnectionKey_NullOrEmpty_ReturnsEmpty()
    {
        var typeSource = new MockTypeSource(typeof(IdNameRow), typeof(KeyedRow));
        var registry = new DefaultRowTypeRegistry(typeSource);

        Assert.Empty(registry.ByConnectionKey(null!));
        Assert.Empty(registry.ByConnectionKey(string.Empty));
    }

    [Fact]
    public void ByConnectionKey_ReturnsOnlyRowsWithMatchingConnectionKey()
    {
        var typeSource = new MockTypeSource(typeof(IdNameRow), typeof(KeyedRow), typeof(OtherKeyedRow));
        var registry = new DefaultRowTypeRegistry(typeSource);

        Assert.Equal(new[] { typeof(KeyedRow) }, registry.ByConnectionKey("TestKey").ToList());
        Assert.Equal(new[] { typeof(OtherKeyedRow) }, registry.ByConnectionKey("OtherKey").ToList());
        Assert.Empty(registry.ByConnectionKey("Missing").ToList());
    }

    [Fact]
    public void MockRowTypeRegistry_AllRowTypes_ReturnsProvidedTypes()
    {
        IRowTypeRegistry registry = new MockRowTypeRegistry(typeof(IdNameRow), typeof(KeyedRow));

        Assert.Equal(new[] { typeof(IdNameRow), typeof(KeyedRow) }, registry.AllRowTypes.ToList());
    }

    [Fact]
    public void MockRowTypeRegistry_ByConnectionKey_FiltersByConnectionKeyAttribute()
    {
        var registry = new MockRowTypeRegistry(typeof(IdNameRow), typeof(KeyedRow), typeof(OtherKeyedRow));

        Assert.Equal(new[] { typeof(KeyedRow) }, registry.ByConnectionKey("TestKey").ToList());
        Assert.Empty(registry.ByConnectionKey("Missing").ToList());
    }
}
