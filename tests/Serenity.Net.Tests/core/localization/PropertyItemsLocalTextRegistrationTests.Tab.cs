using Serenity.Localization;

namespace Serenity.Tests.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    [FormScript]
    private class TabWithTextKeys
    {
        [Tab("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Tab("Columns.SomeColumns.SomeField")]
        public string Cols { get; set; }

        [Tab("Forms.SomeForm.SomeKey")]
        public string Forms { get; set; }

        [Tab("Site.AKey")]
        public string Site { get; set; }

        [Tab("Site.ExistingKey")]
        public string Existing { get; set; }
    }

    [Fact]
    public void Tab_With_Regular_Keys_Are_Registered_As_Null()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID, "Site.ExistingKey", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(TabWithTextKeys));

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
    private class TabWithNonSupportedKeyLike
    {
        [Tab("S.No")]
        public string SNo { get; set; }

        [Tab("DB.ID")]
        public string DBID { get; set; }

        [Tab("New.Value")]
        public string ExistingKey { get; set; }
    }

    [Fact]
    public void Tab_With_NonSupported_KeyLike_Are_Registered_As_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Columns.TestCols.Tabs.New.Value", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(TabWithNonSupportedKeyLike));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Columns.TestCols.Tabs.DB.ID", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("DB.ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.Tabs.New.Value", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("New.Value", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.Tabs.S.No", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("S.No", x.Value);
            });
    }

    [FormScript("MyForm")]
    private class TabWithRegularText
    {
        [Tab("Starting Date")]
        public string StartDate { get; set; }

        [Tab("Some ID")]
        public string SomeID { get; set; }

        [Tab("Date of Birth")]
        public string DOB { get; set; }
    }

    [Fact]
    public void Tab_With_RegularText_Are_Registered_With_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Forms.MyForm.Tabs.Some ID", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(TabWithRegularText));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Forms.MyForm.Tabs.Date of Birth", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Date of Birth", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.Tabs.Some ID", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Some ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.Tabs.Starting Date", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Starting Date", x.Value);
            });
    }
}