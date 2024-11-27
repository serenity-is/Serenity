using Serenity.Localization;

namespace Serenity.Tests.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    [FormScript]
    private class HintWithTextKeys
    {
        [Hint("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Hint("Columns.SomeColumns.SomeField")]
        public string Cols { get; set; }

        [Hint("Forms.SomeForm.SomeKey")]
        public string Forms { get; set; }

        [Hint("Site.AKey")]
        public string Site { get; set; }

        [Hint("Site.ExistingKey")]
        public string Existing { get; set; }
    }

    [Fact]
    public void Hint_With_Regular_Keys_Are_Registered_As_Null()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID, "Site.ExistingKey", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(HintWithTextKeys));

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
    private class HintWithNonSupportedKeyLike
    {
        [Hint("S.No")]
        public string SNo { get; set; }

        [Hint("DB.ID")]
        public string DBID { get; set; }

        [Hint("New.Value")]
        public string ExistingKey { get; set; }
    }

    [Fact]
    public void Hint_With_NonSupported_KeyLike_Are_Registered_As_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Columns.TestCols.ExistingKey_Hint", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(HintWithNonSupportedKeyLike));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Columns.TestCols.DBID_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("DB.ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.ExistingKey_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("New.Value", x.Value);
            },
            x =>
            {
                Assert.Equal("Columns.TestCols.SNo_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("S.No", x.Value);
            });
    }

    [FormScript("MyForm")]
    private class HintWithRegularText
    {
        [Hint("Starting Date")]
        public string StartDate { get; set; }

        [Hint("Some ID")]
        public string SomeID { get; set; }

        [Hint("Date of Birth")]
        public string DOB { get; set; }
    }

    [Fact]
    public void Hint_With_RegularText_Are_Registered_With_Their_Values()
    {
        var registry = new LocalTextRegistry();
        registry.Add(LocalText.InvariantLanguageID,
            "Forms.MyForm.SomeID_Hint", "ExistingValue");

        var typeSource = new MockTypeSource(typeof(HintWithRegularText));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Collection(registry.GetAllTexts(false).OrderBy(x => x.Key.Key),
            x =>
            {
                Assert.Equal("Forms.MyForm.DOB_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Date of Birth", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.SomeID_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Some ID", x.Value);
            },
            x =>
            {
                Assert.Equal("Forms.MyForm.StartDate_Hint", x.Key.Key);
                Assert.Equal(LocalText.InvariantLanguageID, x.Key.LanguageId);
                Assert.Equal("Starting Date", x.Value);
            });
    }
}