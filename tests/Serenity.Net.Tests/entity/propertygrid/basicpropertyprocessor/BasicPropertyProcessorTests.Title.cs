using Serenity.Localization;
using Serenity.PropertyGrid;

namespace Serenity.Tests.Entity;

public partial class BasicPropertyProcessorTests
{
    private class DisplayWithTextKeys
    {
        [DisplayName("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [DisplayName("Columns.SomeColumns.SomeField")]
        public string Cols { get; set; }

        [DisplayName("Forms.SomeForm.SomeKey")]
        public string Forms { get; set; }

        [DisplayName("Site.SomeKey")]
        public string Site { get; set; }
    }

    [Theory]
    [InlineData(nameof(DisplayWithTextKeys.Db), "Db.SomeEntity.SomeField")]
    [InlineData(nameof(DisplayWithTextKeys.Cols), "Columns.SomeColumns.SomeField")]
    [InlineData(nameof(DisplayWithTextKeys.Forms), "Forms.SomeForm.SomeKey")]
    [InlineData(nameof(DisplayWithTextKeys.Site), "Site.SomeKey")]
    public void Title_Should_Use_DisplayNameAttribute_With_LocalTextKey(string propertyName, string expectedKey)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(DisplayWithTextKeys)
            .GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedKey, item.Title);
    }

    [ColumnsScript]
    private class DisplayWithNonSupportedKeyLike
    {
        [DisplayName("S.No")]
        public string SNo { get; set; }

        [DisplayName("Inv.Number")]
        public string Forms { get; set; }

        [DisplayName("DB.ID")]
        public string DBID { get; set; }
    }

    [Theory]
    [InlineData(nameof(DisplayWithNonSupportedKeyLike.SNo))]
    [InlineData(nameof(DisplayWithNonSupportedKeyLike.Forms))]
    [InlineData(nameof(DisplayWithNonSupportedKeyLike.DBID))]
    public void Title_Should_Consider_DisplayName_With_Unsupported_Prefixes_As_Text(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(DisplayWithNonSupportedKeyLike).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Columns.{typeof(DisplayWithNonSupportedKeyLike).FullName}.{propertyName}",
            item.Title);
    }

    private class DisplayWithNonSupportedKeyLikeNoAttr
    {
        [DisplayName("S.No")]
        public string SNo { get; set; }
    }

    [Fact]
    public void Title_Should_Use_TextsAsIs_For_Types_Without_ColumnScript_And_FormScript()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var propertyName = nameof(DisplayWithNonSupportedKeyLikeNoAttr.SNo);
        var property = typeof(DisplayWithNonSupportedKeyLikeNoAttr)
            .GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        // display name is not possibly registered as local text so uses the text as is
        Assert.Equal("S.No", item.Title);
    }

    [FormScript]
    private class DisplayWithRegularText
    {
        [DisplayName("Starting Date")]
        public string StartDate { get; set; }

        [DisplayName("ID")]
        public string SomeID { get; set; }

        [DisplayName("Date of Birth")]
        public string DOB { get; set; }
    }

    [Theory]
    [InlineData(nameof(DisplayWithRegularText.StartDate))]
    [InlineData(nameof(DisplayWithRegularText.SomeID))]
    [InlineData(nameof(DisplayWithRegularText.DOB))]
    public void Title_Should_Use_DisplayNameAttribute_With_Regular_Text_AsIs(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(DisplayWithRegularText).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Forms.{typeof(DisplayWithRegularText).FullName}.{propertyName}",
            item.Title);
    }

    [LocalTextPrefix("MyRow")]
    public class DisplayWithRowRow : Row<DisplayWithRowRow.RowFields>
    {
        public string FormOnly { get; set; }

        public string FormOnlyTextKey { get; set; }

        [DisplayName("O")]
        public string FormOverride { get; set; }

        [DisplayName("Site.CustomRowTextKey")]
        public string FormOverrideTextKey { get; set; }

        [DisplayName("R")]
        public string RowOnly { get; set; }

        [DisplayName("Site.CustomRowOnlyTextKey")]
        public string RowOnlyTextKey { get; set; }

        public string WithEmptyStringInForm { get; set; }

        [DisplayName("")]
        public string WithEmptyStringInForm2 { get; set; }

        [DisplayName(null)]
        public string WithNullStringInForm { get; set; }

        [DisplayName(null)]
        public string WithNullStringInForm2 { get; set; }

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

    [FormScript("MyForm"), BasedOnRow(typeof(DisplayWithRowRow))]
    private class DisplayWithRowForm
    {
        [DisplayName("A")]
        public string NotMapped { get; set; }

        [DisplayName("B")]
        public string FormOnly { get; set; }

        [DisplayName("Site.FormOnlyTextKey")]
        public string FormOnlyTextKey { get; set; }

        [DisplayName("C")]
        public string FormOverride { get; set; }

        [DisplayName("Site.FormCustomTextKeyOverride")]
        public string FormOverrideTextKey { get; set; }

        public string RowOnly { get; set; }

        public string RowOnlyTextKey { get; set; }

        [DisplayName("")]
        public string WithEmptyStringInForm { get; set; }

        [DisplayName("")]
        public string WithEmptyStringInForm2 { get; set; }

        [DisplayName(null)]
        public string WithNullStringInForm { get; set; }

        [DisplayName(null)]
        public string WithNullStringInForm2 { get; set; }
    }

    [Theory]
    [InlineData("NotMapped", "Forms.MyForm.NotMapped")]
    [InlineData("FormOnly", "Forms.MyForm.FormOnly")]
    [InlineData("FormOnlyTextKey", "Site.FormOnlyTextKey")]
    [InlineData("FormOverride", "Forms.MyForm.FormOverride")]
    [InlineData("FormOverrideTextKey", "Site.FormCustomTextKeyOverride")]
    [InlineData("RowOnly", "Db.MyRow.RowOnly")]
    [InlineData("RowOnlyTextKey", "Site.CustomRowOnlyTextKey")]
    [InlineData("WithEmptyStringInForm", "")]
    [InlineData("WithEmptyStringInForm2", "")]
    [InlineData("WithNullStringInForm", null)]
    [InlineData("WithNullStringInForm2", null)]
    public void Title_Should_Use_Row_Properties_If_Available(string propertyName,
        string key)
    {
        var processor = new BasicPropertyProcessor();

        var registry = new LocalTextRegistry();
        EntityLocalTexts.AddRowTexts(registry, new[] { new DisplayWithRowRow() });

        var item = new PropertyItem();
        var property = typeof(DisplayWithRowForm).GetProperty(propertyName);
        var source = new PropertyInfoSource(property,
            new DisplayWithRowRow());

        processor.Process(source, item);

        Assert.Equal(key, item.Title);
    }
}