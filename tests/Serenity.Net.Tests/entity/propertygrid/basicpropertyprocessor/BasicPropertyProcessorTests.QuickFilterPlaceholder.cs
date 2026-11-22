namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [ColumnsScript]
    private class QuickFilterPlaceholderColumns
    {
        [QuickFilter(Placeholder = "Any city")]
        public string City { get; set; }

        [QuickFilter(Placeholder = "Site.LooksLikeAKey")]
        public string KeyLike { get; set; }

        [QuickFilter(PlaceholderKey = "Site.CountryPlaceholder")]
        public string Country { get; set; }

        [QuickFilter(Placeholder = "Any region", PlaceholderKey = "Site.RegionPlaceholder")]
        public string Region { get; set; }

        [QuickFilter(Placeholder = "")]
        public string Empty { get; set; }

        [QuickFilter, Placeholder("Form placeholder")]
        public string FormOnly { get; set; }
    }

    private class QuickFilterPlaceholderNoScript
    {
        [QuickFilter(Placeholder = "Any city")]
        public string City { get; set; }
    }

    private static PropertyItem ProcessQuickFilterPlaceholder(Type type, string propertyName)
    {
        var item = new PropertyItem();
        new BasicPropertyProcessor().Process(new PropertyInfoSource(type.GetProperty(propertyName), null), item);
        return item;
    }

    [Fact]
    public void QuickFilter_Placeholder_Uses_The_Key_The_Property_Implies()
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderColumns), nameof(QuickFilterPlaceholderColumns.City));
        Assert.Equal($"Columns.{typeof(QuickFilterPlaceholderColumns).FullName}.City_QuickFilter_Placeholder",
            item.QuickFilterParams["placeholder"]);
    }

    [Fact]
    public void QuickFilter_Placeholder_Is_Never_Taken_As_A_Key()
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderColumns), nameof(QuickFilterPlaceholderColumns.KeyLike));
        Assert.Equal($"Columns.{typeof(QuickFilterPlaceholderColumns).FullName}.KeyLike_QuickFilter_Placeholder",
            item.QuickFilterParams["placeholder"]);
    }

    [Theory]
    [InlineData(nameof(QuickFilterPlaceholderColumns.Country), "Site.CountryPlaceholder")]
    [InlineData(nameof(QuickFilterPlaceholderColumns.Region), "Site.RegionPlaceholder")]
    public void QuickFilter_PlaceholderKey_Is_Used_Instead_Of_The_Implied_Key(string propertyName, string expected)
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderColumns), propertyName);
        Assert.Equal(expected, item.QuickFilterParams["placeholder"]);
    }

    [Fact]
    public void QuickFilter_Empty_Placeholder_Is_Kept_As_Is()
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderColumns), nameof(QuickFilterPlaceholderColumns.Empty));
        Assert.Equal("", item.QuickFilterParams["placeholder"]);
    }

    [Fact]
    public void QuickFilter_Placeholder_Is_Not_Set_From_The_Placeholder_Of_The_Property()
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderColumns), nameof(QuickFilterPlaceholderColumns.FormOnly));
        Assert.False(item.QuickFilterParams.ContainsKey("placeholder"));
        Assert.NotNull(item.Placeholder);
    }

    [Fact]
    public void QuickFilter_Placeholder_Is_The_Text_For_A_Type_Without_A_Script()
    {
        var item = ProcessQuickFilterPlaceholder(typeof(QuickFilterPlaceholderNoScript), nameof(QuickFilterPlaceholderNoScript.City));
        Assert.Equal("Any city", item.QuickFilterParams["placeholder"]);
    }
}
