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

    [FormScript("CategoryKeyForm")]
    private class CategoryKeyDecorated
    {
        [CategoryName("Invariant category", TextKey = "Site.CategoryKey")]
        public string ExplicitKey { get; set; }

        [CategoryName("Site.LiteralCategory")]
        public string KeyShapedLiteral { get; set; }
    }

    [FormScript("TabKeyForm")]
    private class TabKeyDecorated
    {
        [Tab("Invariant tab", TextKey = "Site.TabKey")]
        public string ExplicitKey { get; set; }

        [Tab("Site.LiteralTab")]
        public string KeyShapedLiteral { get; set; }
    }

    [FormScript("HintPlaceholderKeyForm")]
    private class HintPlaceholderKeyDecorated
    {
        [Hint("Invariant hint", TextKey = "Site.HintKey")]
        [Placeholder("Invariant placeholder", TextKey = "Site.PlaceholderKey")]
        public string ExplicitKey { get; set; }

        [Hint("Site.LiteralHint")]
        [Placeholder("Site.LiteralPlaceholder")]
        public string KeyShapedLiteral { get; set; }
    }

    [TableName("PGRows")]
    private class PGRow : Row<PGRow.RowFields>, IIdRow
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public StringField Name = null!;
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
    public void Processes_CategoryName_TextKey_And_Does_Not_Infer_Key_Shaped_Text()
    {
        using var provider = CreateProvider(typeof(BasicPropertyProcessor));
        var items = provider.GetPropertyItemsFor(typeof(CategoryKeyDecorated), null)
            .ToDictionary(x => x.Name);

        Assert.Equal("Site.CategoryKey", items[nameof(CategoryKeyDecorated.ExplicitKey)].Category);
        Assert.Equal("Forms.CategoryKeyForm.Categories.Site.LiteralCategory",
            items[nameof(CategoryKeyDecorated.KeyShapedLiteral)].Category);
    }

    [Fact]
    public void Processes_Tab_TextKey_And_Does_Not_Infer_Key_Shaped_Text()
    {
        using var provider = CreateProvider(typeof(BasicPropertyProcessor));
        var items = provider.GetPropertyItemsFor(typeof(TabKeyDecorated), null)
            .ToDictionary(x => x.Name);

        Assert.Equal("Site.TabKey", items[nameof(TabKeyDecorated.ExplicitKey)].Tab);
        Assert.Equal("Forms.TabKeyForm.Tabs.Site.LiteralTab",
            items[nameof(TabKeyDecorated.KeyShapedLiteral)].Tab);
    }

    [Fact]
    public void Processes_Hint_And_Placeholder_TextKeys_Without_Inferring_Key_Shaped_Text()
    {
        using var provider = CreateProvider(typeof(BasicPropertyProcessor));
        var items = provider.GetPropertyItemsFor(typeof(HintPlaceholderKeyDecorated), null)
            .ToDictionary(x => x.Name);

        Assert.Equal("Site.HintKey", items[nameof(HintPlaceholderKeyDecorated.ExplicitKey)].Hint);
        Assert.Equal("Site.PlaceholderKey", items[nameof(HintPlaceholderKeyDecorated.ExplicitKey)].Placeholder);
        Assert.Equal("Forms.HintPlaceholderKeyForm.KeyShapedLiteral_Hint",
            items[nameof(HintPlaceholderKeyDecorated.KeyShapedLiteral)].Hint);
        Assert.Equal("Forms.HintPlaceholderKeyForm.KeyShapedLiteral_Placeholder",
            items[nameof(HintPlaceholderKeyDecorated.KeyShapedLiteral)].Placeholder);
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


