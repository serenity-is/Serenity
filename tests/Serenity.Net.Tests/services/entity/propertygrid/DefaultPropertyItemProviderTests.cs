#pragma warning disable CS0649
namespace Serenity.PropertyGrid;

public class DefaultPropertyItemProviderTests
{
    private static DefaultPropertyItemProvider CreateProvider(params Type[] types)
    {
        var services = new ServiceCollection();
        services.AddOptions();
        var provider = services.BuildServiceProvider();
        return new DefaultPropertyItemProvider(provider, new MockTypeSource(types));
    }

    private class Sample
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        [IgnoreUIField]
        public string Ignored { get; set; }
    }

    private class Decorated
    {
        [DisplayName("N")]
        [Hint("H")]
        [Placeholder("P")]
        [Category("C")]
        [Tab("T")]
        public string Name { get; set; }
    }

    [TableName("PGRows")]
    private class PGRow : Row<PGRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID;
            public StringField Name;
        }
    }

    [BasedOnRow(typeof(PGRow), CheckNames = true)]
    private class MatchingForm
    {
        public string Name { get; set; }
    }

    [BasedOnRow(typeof(PGRow), CheckNames = true)]
    private class MismatchingForm
    {
        public string Other { get; set; }
    }

    [BasedOnRow(typeof(string))]
    private class InvalidBasedOnRowForm
    {
        public string Name { get; set; }
    }

    [Fact]
    public void Throws_ForNullProvider()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultPropertyItemProvider(null!, new MockTypeSource()));
    }

    [Fact]
    public void Throws_ForNullTypeSource()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new DefaultPropertyItemProvider(services, null!));
    }

    [Fact]
    public void Throws_ForNullType()
    {
        using var provider = CreateProvider();
        Assert.Throws<ArgumentNullException>(() => provider.GetPropertyItemsFor(null!, null).ToList());
    }

    [Fact]
    public void ReturnsItems_ForProperties()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(Sample), null).ToList();

        Assert.Equal(2, items.Count);
        Assert.Contains(items, x => x.Name == "Name");
        Assert.Contains(items, x => x.Name == "Age");
    }

    [Fact]
    public void SkipsPropertiesWithIgnoreUIField()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(Sample), null).ToList();
        Assert.DoesNotContain(items, x => x.Name == "Ignored");
    }

    [Fact]
    public void AppliesPredicate()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(Sample), p => p.Name == "Name").ToList();
        Assert.Single(items);
        Assert.Equal("Name", items[0].Name);
    }

    [Fact]
    public void ProcessesWithBasicPropertyProcessor()
    {
        using var provider = CreateProvider(typeof(BasicPropertyProcessor));
        var items = provider.GetPropertyItemsFor(typeof(Decorated), null).ToList();

        var item = Assert.Single(items);
        Assert.Equal("N", item.Title);
        Assert.Equal("H", item.Hint);
        Assert.Equal("P", item.Placeholder);
        Assert.Equal("C", item.Category);
        Assert.Equal("T", item.Tab);
    }

    [Fact]
    public void CheckNames_Passes_WhenFieldMatches()
    {
        using var provider = CreateProvider();
        var items = provider.GetPropertyItemsFor(typeof(MatchingForm), null).ToList();
        Assert.Single(items);
    }

    [Fact]
    public void CheckNames_Throws_WhenFieldDoesNotMatch()
    {
        using var provider = CreateProvider();
        Assert.Throws<InvalidProgramException>(() =>
            provider.GetPropertyItemsFor(typeof(MismatchingForm), null).ToList());
    }

    [Fact]
    public void Throws_ForInvalidBasedOnRowType()
    {
        using var provider = CreateProvider();
        Assert.Throws<InvalidOperationException>(() =>
            provider.GetPropertyItemsFor(typeof(InvalidBasedOnRowForm), null).ToList());
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var provider = CreateProvider();
        provider.Dispose();
    }
}


