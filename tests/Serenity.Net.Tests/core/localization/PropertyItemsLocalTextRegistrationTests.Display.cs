using Serenity.Localization;

namespace Serenity.Tests.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    [FormScript]
    private class DisplayWithTextKeys
    {
        [DisplayName("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [DisplayName("Columns.SomeColumns.SomeField")]
        public string Cols { get; set; }

        [DisplayName("Forms.SomeForm.SomeKey")]
        public string Forms { get; set; }

        [DisplayName("Site.AKey")]
        public string Site { get; set; }

        [DisplayName("Site.ExistingKey")]
        public string Existing { get; set; }
    }

    [Fact]
    public void Display_With_Regular_Keys_Are_Registered_As_Null()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID, "Site.ExistingKey", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(DisplayWithTextKeys));

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
    private class DisplayWithNonSupportedKeyLike
    {
        [DisplayName("S.No")]
        public string SNo { get; set; }

        [DisplayName("DB.ID")]
        public string DBID { get; set; }

        [DisplayName("New.Value")]
        public string ExistingKey { get; set; }
    }

    [Fact]
    public void Display_With_NonSupported_KeyLike_Are_Registered_As_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Columns.TestCols.ExistingKey", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(DisplayWithNonSupportedKeyLike));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Columns.TestCols.DBID", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("DB.ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.ExistingKey", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("New.Value", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.SNo", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("S.No", x.Value);
            });
    }

    [FormScript("MyForm")]
    private class DisplayWithRegularText
    {
        [DisplayName("Starting Date")]
        public string StartDate { get; set; }

        [DisplayName("Some ID")]
        public string SomeID { get; set; }

        [DisplayName("Date of Birth")]
        public string DOB { get; set; }

        [DisplayName("")]
        public string EmptyString { get; set; }

        [DisplayName(null)]
        public string NullString { get; set; }
    }

    [Fact]
    public void Display_With_RegularText_Are_Registered_With_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Forms.MyForm.SomeID", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(DisplayWithRegularText));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Forms.MyForm.DOB", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Date of Birth", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.SomeID", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Some ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.StartDate", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Starting Date", x.Value);
            });
    }
}