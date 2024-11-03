using Serenity.PropertyGrid;

namespace Serenity.Tests.Entity;

public partial class BasicPropertyProcessorTests
{
    private class PlaceholderWithTextKeys
    {
        [Placeholder("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Placeholder("Columns.SomeColumns.SomeFieldPlaceholder")]
        public string Cols { get; set; }

        [Placeholder("Forms.SomeForm.SomeKeyPlaceholder")]
        public string Forms { get; set; }

        [Placeholder("Site.SomeKey")]
        public string Site { get; set; }
    }

    [Theory]
    [InlineData(nameof(PlaceholderWithTextKeys.Db), "Db.SomeEntity.SomeField")]
    [InlineData(nameof(PlaceholderWithTextKeys.Cols), "Columns.SomeColumns.SomeFieldPlaceholder")]
    [InlineData(nameof(PlaceholderWithTextKeys.Forms), "Forms.SomeForm.SomeKeyPlaceholder")]
    [InlineData(nameof(PlaceholderWithTextKeys.Site), "Site.SomeKey")]
    public void Placeholder_Should_Use_PlaceholderAttribute_With_LocalTextKey(string propertyName, string expectedKey)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(PlaceholderWithTextKeys)
            .GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedKey, item.Placeholder);
    }

    [ColumnsScript]
    private class PlaceholderWithNonSupportedKeyLike
    {
        [Placeholder("S.No")]
        public string SNo { get; set; }

        [Placeholder("Inv.Number")]
        public string Forms { get; set; }

        [Placeholder("DB.ID")]
        public string DBID { get; set; }
    }

    [Theory]
    [InlineData(nameof(PlaceholderWithNonSupportedKeyLike.SNo))]
    [InlineData(nameof(PlaceholderWithNonSupportedKeyLike.Forms))]
    [InlineData(nameof(PlaceholderWithNonSupportedKeyLike.DBID))]
    public void Placeholder_Should_Consider_Placeholder_With_Unsupported_Prefixes_As_Text(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(PlaceholderWithNonSupportedKeyLike).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Columns.{typeof(PlaceholderWithNonSupportedKeyLike).FullName}.{propertyName}_Placeholder",
            item.Placeholder);
    }

    private class PlaceholderWithNonSupportedKeyLikeNoAttr
    {
        [Placeholder("S.No")]
        public string SNo { get; set; }
    }

    [Fact]
    public void Placeholder_Should_Use_TextsAsIs_For_Types_Without_ColumnScript_And_FormScript()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var propertyName = nameof(PlaceholderWithNonSupportedKeyLikeNoAttr.SNo);
        var property = typeof(PlaceholderWithNonSupportedKeyLikeNoAttr)
            .GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        // display name is not possibly registered as local text so uses the text as is
        Assert.Equal("S.No", item.Placeholder);
    }

    [FormScript]
    private class PlaceholderWithRegularText
    {
        [Placeholder("Starting Date")]
        public string StartDate { get; set; }

        [Placeholder("ID")]
        public string SomeID { get; set; }

        [Placeholder("Date of Birth")]
        public string DOB { get; set; }
    }

    [Theory]
    [InlineData(nameof(PlaceholderWithRegularText.StartDate))]
    [InlineData(nameof(PlaceholderWithRegularText.SomeID))]
    [InlineData(nameof(PlaceholderWithRegularText.DOB))]
    public void Placeholder_Should_Use_PlaceholderAttribute_With_Regular_Text_AsIs(
        string propertyName)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(PlaceholderWithRegularText).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Forms.{typeof(PlaceholderWithRegularText).FullName}.{propertyName}_Placeholder",
            item.Placeholder);
    }

    [LocalTextPrefix("MyRow")]
    public class PlaceholderWithRowRow : Row<PlaceholderWithRowRow.RowFields>
    {
        public string FormOnly { get; set; }

        public string FormOnlyTextKey { get; set; }

        [Placeholder("O")]
        public string FormOverride { get; set; }

        [Placeholder("Site.CustomRowTextKey")]
        public string FormOverrideTextKey { get; set; }

        [Placeholder("R")]
        public string RowOnly { get; set; }

        [Placeholder("Site.CustomRowOnlyTextKey")]
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

    [FormScript("MyForm"), BasedOnRow(typeof(PlaceholderWithRowRow))]
    private class PlaceholderWithRowForm
    {
        [Placeholder("A")]
        public string NotMapped { get; set; }

        [Placeholder("B")]
        public string FormOnly { get; set; }

        [Placeholder("Site.FormOnlyTextKey")]
        public string FormOnlyTextKey { get; set; }

        [Placeholder("C")]
        public string FormOverride { get; set; }

        [Placeholder("Site.FormCustomTextKeyOverride")]
        public string FormOverrideTextKey { get; set; }

        public string RowOnly { get; set; }

        public string RowOnlyTextKey { get; set; }
    }

    [Theory]
    [InlineData("NotMapped", "Forms.MyForm.NotMapped_Placeholder")]
    [InlineData("FormOnly", "Forms.MyForm.FormOnly_Placeholder")]
    [InlineData("FormOnlyTextKey", "Site.FormOnlyTextKey")]
    [InlineData("FormOverride", "Forms.MyForm.FormOverride_Placeholder")]
    [InlineData("FormOverrideTextKey", "Site.FormCustomTextKeyOverride")]
    [InlineData("RowOnly", "Db.MyRow.RowOnly_Placeholder")]
    [InlineData("RowOnlyTextKey", "Site.CustomRowOnlyTextKey")]
    public void Placeholder_Should_Use_Row_Properties_If_Available(string propertyName,
        string key)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(PlaceholderWithRowForm).GetProperty(propertyName);
        var source = new PropertyInfoSource(property,
            new PlaceholderWithRowRow());

        processor.Process(source, item);

        Assert.Equal(key, item.Placeholder);
    }
}