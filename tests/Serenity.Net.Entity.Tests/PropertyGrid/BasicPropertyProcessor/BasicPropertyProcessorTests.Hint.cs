using Serenity.Localization;
using Serenity.PropertyGrid;

namespace Serenity.Tests.Entity;

public partial class BasicPropertyProcessorTests
{
    private class HintWithTextKeys
    {
        [Hint("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Hint("Columns.SomeColumns.SomeFieldHint")]
        public string Cols { get; set; }

        [Hint("Forms.SomeForm.SomeKeyHint")]
        public string Forms { get; set; }

        [Hint("Site.SomeKey")]
        public string Site { get; set; }
    }

    [Theory]
    [InlineData(nameof(HintWithTextKeys.Db), "Db.SomeEntity.SomeField")]
    [InlineData(nameof(HintWithTextKeys.Cols), "Columns.SomeColumns.SomeFieldHint")]
    [InlineData(nameof(HintWithTextKeys.Forms), "Forms.SomeForm.SomeKeyHint")]
    [InlineData(nameof(HintWithTextKeys.Site), "Site.SomeKey")]
    public void Hint_Should_Use_HintAttribute_With_LocalTextKey(string propertyName, string expectedKey)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(HintWithTextKeys)
            .GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedKey, item.Hint);
    }

    [ColumnsScript]
    private class HintWithNonSupportedKeyLike
    {
        [Hint("S.No")]
        public string SNo { get; set; }

        [Hint("Inv.Number")]
        public string Forms { get; set; }

        [Hint("DB.ID")]
        public string DBID { get; set; }
    }

    [Theory]
    [InlineData(nameof(HintWithNonSupportedKeyLike.SNo))]
    [InlineData(nameof(HintWithNonSupportedKeyLike.Forms))]
    [InlineData(nameof(HintWithNonSupportedKeyLike.DBID))]
    public void Hint_Should_Consider_Hint_With_Unsupported_Prefixes_As_Text(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(HintWithNonSupportedKeyLike).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Columns.{typeof(HintWithNonSupportedKeyLike).FullName}.{propertyName}_Hint",
            item.Hint);
    }

    private class HintWithNonSupportedKeyLikeNoAttr
    {
        [Hint("S.No")]
        public string SNo { get; set; }
    }

    [Fact]
    public void Hint_Should_Use_TextsAsIs_For_Types_Without_ColumnScript_And_FormScript()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var propertyName = nameof(HintWithNonSupportedKeyLikeNoAttr.SNo);
        var property = typeof(HintWithNonSupportedKeyLikeNoAttr)
            .GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        // display name is not possibly registered as local text so uses the text as is
        Assert.Equal("S.No", item.Hint);
    }

    [FormScript]
    private class HintWithRegularText
    {
        [Hint("Starting Date")]
        public string StartDate { get; set; }

        [Hint("ID")]
        public string SomeID { get; set; }

        [Hint("Date of Birth")]
        public string DOB { get; set; }
    }

    [Theory]
    [InlineData(nameof(HintWithRegularText.StartDate))]
    [InlineData(nameof(HintWithRegularText.SomeID))]
    [InlineData(nameof(HintWithRegularText.DOB))]
    public void Hint_Should_Use_HintAttribute_With_Regular_Text_AsIs(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(HintWithRegularText).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Forms.{typeof(HintWithRegularText).FullName}.{propertyName}_Hint",
            item.Hint);
    }

    [LocalTextPrefix("MyRow")]
    public class HintWithRowRow : Row<HintWithRowRow.RowFields>
    {
        public string FormOnly { get; set; }

        public string FormOnlyTextKey { get; set; }

        [Hint("O")]
        public string FormOverride { get; set; }

        [Hint("Site.CustomRowTextKey")]
        public string FormOverrideTextKey { get; set; }

        [Hint("R")]
        public string RowOnly { get; set; }

        [Hint("Site.CustomRowOnlyTextKey")]
        public string RowOnlyTextKey { get; set; }

        public class RowFields : RowFieldsBase
        {
            public StringField FormOnly;
            public StringField FormOnlyTextKey;
            public StringField FormOverride;
            public StringField FormOverrideTextKey;
            public StringField RowOnly;
            public StringField RowOnlyTextKey;
        }
    }

    [FormScript("MyForm"), BasedOnRow(typeof(HintWithRowRow))]
    private class HintWithRowForm
    {
        [Hint("A")]
        public string NotMapped { get; set; }

        [Hint("B")]
        public string FormOnly { get; set; }

        [Hint("Site.FormOnlyTextKey")]
        public string FormOnlyTextKey { get; set; }

        [Hint("C")]
        public string FormOverride { get; set; }

        [Hint("Site.FormCustomTextKeyOverride")]
        public string FormOverrideTextKey { get; set; }

        public string RowOnly { get; set; }

        public string RowOnlyTextKey { get; set; }
    }

    [Theory]
    [InlineData("NotMapped", "Forms.MyForm.NotMapped_Hint")]
    [InlineData("FormOnly", "Forms.MyForm.FormOnly_Hint")]
    [InlineData("FormOnlyTextKey", "Site.FormOnlyTextKey")]
    [InlineData("FormOverride", "Forms.MyForm.FormOverride_Hint")]
    [InlineData("FormOverrideTextKey", "Site.FormCustomTextKeyOverride")]
    [InlineData("RowOnly", "Db.MyRow.RowOnly_Hint")]
    [InlineData("RowOnlyTextKey", "Site.CustomRowOnlyTextKey")]
    public void Hint_Should_Use_Row_Properties_If_Available(string propertyName,
        string key)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(HintWithRowForm).GetProperty(propertyName);
        var source = new PropertyInfoSource(property,
            new HintWithRowRow());

        processor.Process(source, item);

        Assert.Equal(key, item.Hint);
    }
}