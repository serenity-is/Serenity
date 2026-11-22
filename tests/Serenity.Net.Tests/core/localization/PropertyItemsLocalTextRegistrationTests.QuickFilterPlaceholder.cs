namespace Serenity.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    [ColumnsScript("QuickFilterPlaceholders")]
    private class QuickFilterPlaceholders
    {
        [QuickFilter(Placeholder = "Any city")]
        public string City { get; set; }

        [QuickFilter(Placeholder = "Site.LooksLikeAKey")]
        public string KeyLike { get; set; }

        [QuickFilter(PlaceholderKey = "Site.CountryPlaceholder")]
        public string Country { get; set; }

        [QuickFilter(Placeholder = "Any region", PlaceholderKey = "Site.RegionPlaceholder")]
        public string Region { get; set; }

        [QuickFilter]
        public string NoPlaceholder { get; set; }
    }

    [Fact]
    public void QuickFilter_Placeholders_Are_Registered_With_Their_Keys()
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry,
            new MockTypeSource(typeof(QuickFilterPlaceholders)));

        var texts = registry.GetAllTexts(false)
            .Where(x => x.Key.Key.Contains("QuickFilter_Placeholder") || x.Key.Key.StartsWith("Site."))
            .ToDictionary(x => x.Key.Key, x => x.Value);

        Assert.Equal(new Dictionary<string, string>
        {
            ["Columns.QuickFilterPlaceholders.City_QuickFilter_Placeholder"] = "Any city",
            ["Columns.QuickFilterPlaceholders.KeyLike_QuickFilter_Placeholder"] = "Site.LooksLikeAKey",
            ["Site.CountryPlaceholder"] = null,
            ["Site.RegionPlaceholder"] = "Any region"
        }, texts);
    }
}
