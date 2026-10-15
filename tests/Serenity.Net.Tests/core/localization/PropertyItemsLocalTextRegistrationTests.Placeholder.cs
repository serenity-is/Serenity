namespace Serenity.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    [FormScript]
    private class PlaceholderWithTextKeys
    {
        [Placeholder(TextKey = "Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Placeholder(TextKey = "Columns.SomeColumns.SomeField")]
        public string Cols { get; set; }

        [Placeholder(TextKey = "Forms.SomeForm.SomeKey")]
        public string Forms { get; set; }

        [Placeholder(TextKey = "Site.AKey")]
        public string Site { get; set; }

        [Placeholder(TextKey = "Site.ExistingKey")]
        public string Existing { get; set; }
    }

    [FormScript]
    private class PlaceholderWithExplicitTextKey
    {
        [Placeholder("Regular placeholder", TextKey = "Site.ExplicitPlaceholder")]
        public string Name { get; set; }
    }

    [Fact]
    public void Placeholder_With_Explicit_TextKey_Uses_Explicit_Key()
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry,
            new MockTypeSource(typeof(PlaceholderWithExplicitTextKey)));

        Assert.Contains(registry.GetAllTexts(false),
            x => x.Key.Key == "Site.ExplicitPlaceholder" && x.Value == "Regular placeholder");
    }

    [FormScript("PlaceholderKeyLikeLiteral")]
    private class PlaceholderWithKeyLikeLiteral
    {
        [Placeholder("Site.LegacyPlaceholderKey")]
        public string Name { get; set; }
    }

    [Fact]
    public void Placeholder_With_KeyLike_Literal_Uses_Generated_Key()
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry,
            new MockTypeSource(typeof(PlaceholderWithKeyLikeLiteral)));

        Assert.Contains(registry.GetAllTexts(false),
            x => x.Key.Key == "Forms.PlaceholderKeyLikeLiteral.Name_Placeholder" &&
                x.Value == "Site.LegacyPlaceholderKey");
        Assert.DoesNotContain(registry.GetAllTexts(false),
            x => x.Key.Key == "Site.LegacyPlaceholderKey");
    }

    [Fact]
    public void Placeholder_With_Explicit_Keys_Are_Registered_As_Null()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID, "Site.ExistingKey", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(PlaceholderWithTextKeys));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Columns.SomeColumns.SomeField", x.Key.Key);
                Assert.Null(x.Value);
            },
            x =>
            {
                Assert.Equal("Db.SomeEntity.SomeField", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Null(x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.SomeForm.SomeKey", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Null(x.Value);
            },
            x =>
            {
                Assert.Equal("Site.AKey", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Null(x.Value);
            },
            x =>
            {
                Assert.Equal("Site.ExistingKey", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("ExistingValue", x.Value);
            }
        );
    }

    [ColumnsScript("TestCols")]
    private class PlaceholderWithNonSupportedKeyLike
    {
        [Placeholder("S.No")]
        public string SNo { get; set; }

        [Placeholder("DB.ID")]
        public string DBID { get; set; }

        [Placeholder("New.Value")]
        public string ExistingKey { get; set; }
    }

    [Fact]
    public void Placeholder_With_NonSupported_KeyLike_Are_Registered_As_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Columns.TestCols.ExistingKey_Placeholder", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(PlaceholderWithNonSupportedKeyLike));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Columns.TestCols.DBID_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("DB.ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.ExistingKey_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("New.Value", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.SNo_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("S.No", x.Value);
            });
    }

    [FormScript("MyForm")]
    private class PlaceholderWithRegularText
    {
        [Placeholder("Starting Date")]
        public string StartDate { get; set; }

        [Placeholder("Some ID")]
        public string SomeID { get; set; }

        [Placeholder("Date of Birth")]
        public string DOB { get; set; }
    }

    [Fact]
    public void Placeholder_With_RegularText_Are_Registered_With_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Forms.MyForm.SomeID_Placeholder", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(PlaceholderWithRegularText));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Forms.MyForm.DOB_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Date of Birth", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.SomeID_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Some ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.StartDate_Placeholder", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Starting Date", x.Value);
            });
    }
}