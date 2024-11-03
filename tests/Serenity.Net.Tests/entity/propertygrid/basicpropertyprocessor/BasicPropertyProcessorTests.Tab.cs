using Serenity.PropertyGrid;

namespace Serenity.Tests.Entity;

public partial class BasicPropertyProcessorTests
{
    private class TabWithTextKeys
    {
        [Tab("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Tab("Columns.SomeColumns.SomeFieldTab")]
        public string Cols { get; set; }

        [Tab("Forms.SomeForm.SomeKeyTab")]
        public string Forms { get; set; }

        [Tab("Site.SomeKey")]
        public string Site { get; set; }
    }

    [Theory]
    [InlineData(nameof(TabWithTextKeys.Db), "Db.SomeEntity.SomeField")]
    [InlineData(nameof(TabWithTextKeys.Cols), "Columns.SomeColumns.SomeFieldTab")]
    [InlineData(nameof(TabWithTextKeys.Forms), "Forms.SomeForm.SomeKeyTab")]
    [InlineData(nameof(TabWithTextKeys.Site), "Site.SomeKey")]
    public void Tab_Should_Use_TabAttribute_With_LocalTextKey(string propertyName, string expectedKey)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(TabWithTextKeys)
            .GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedKey, item.Tab);
    }

    [ColumnsScript]
    private class TabWithNonSupportedKeyLike
    {
        [Tab("S.No")]
        public string SNo { get; set; }

        [Tab("Inv.Number")]
        public string Forms { get; set; }

        [Tab("DB.ID")]
        public string DBID { get; set; }
    }

    [Theory]
    [InlineData(nameof(TabWithNonSupportedKeyLike.SNo), "S.No")]
    [InlineData(nameof(TabWithNonSupportedKeyLike.Forms), "Inv.Number")]
    [InlineData(nameof(TabWithNonSupportedKeyLike.DBID), "DB.ID")]
    public void Tab_Should_Consider_Tab_With_Unsupported_Prefixes_As_Text(
        string propertyName, string tab)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(TabWithNonSupportedKeyLike).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Columns.{typeof(TabWithNonSupportedKeyLike).FullName}.Tabs.{tab}",
            item.Tab);
    }

    private class TabWithNonSupportedKeyLikeNoAttr
    {
        [Tab("S.No")]
        public string SNo { get; set; }
    }

    [Fact]
    public void Tab_Should_Use_TextsAsIs_For_Types_Without_ColumnScript_And_FormScript()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var propertyName = nameof(TabWithNonSupportedKeyLikeNoAttr.SNo);
        var property = typeof(TabWithNonSupportedKeyLikeNoAttr)
            .GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        // display name is not possibly registered as local text so uses the text as is
        Assert.Equal("S.No", item.Tab);
    }

    [FormScript]
    private class TabWithRegularText
    {
        [Tab("Starting Date")]
        public string StartDate { get; set; }

        [Tab("ID")]
        public string SomeID { get; set; }

        [Tab("Date of Birth")]
        public string DOB { get; set; }
    }

    [Theory]
    [InlineData(nameof(TabWithRegularText.StartDate), "Starting Date")]
    [InlineData(nameof(TabWithRegularText.SomeID), "ID")]
    [InlineData(nameof(TabWithRegularText.DOB), "Date of Birth")]
    public void Tab_Should_Use_TabAttribute_With_Regular_Text_AsIs(
        string propertyName, string tab)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(TabWithRegularText).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Forms.{typeof(TabWithRegularText).FullName}.Tabs.{tab}",
            item.Tab);
    }

    [LocalTextPrefix("MyRow")]
    public class TabWithRowRow : Row<TabWithRowRow.RowFields>
    {
        public string FormOnly { get; set; }

        public string FormOnlyTextKey { get; set; }

        [Tab("O")]
        public string FormOverride { get; set; }

        [Tab("Site.CustomRowTextKey")]
        public string FormOverrideTextKey { get; set; }

        [Tab("R")]
        public string RowOnly { get; set; }

        [Tab("Site.CustomRowOnlyTextKey")]
        public string RowOnlyTextKey { get; set; }

        public string EmptyString { get; set; }

        [Tab("")]
        public string EmptyString2 { get; set; }

        public string NullString { get; set; }

        [Tab(null)]
        public string NullString2 { get; set; }

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

    [FormScript("MyForm"), BasedOnRow(typeof(TabWithRowRow))]
    private class TabWithRowForm
    {
        [Tab("A")]
        public string NotMapped { get; set; }

        [Tab("B")]
        public string FormOnly { get; set; }

        [Tab("Site.FormOnlyTextKey")]
        public string FormOnlyTextKey { get; set; }

        [Tab("C")]
        public string FormOverride { get; set; }

        [Tab("Site.FormCustomTextKeyOverride")]
        public string FormOverrideTextKey { get; set; }

        public string RowOnly { get; set; }

        public string RowOnlyTextKey { get; set; }

        [Tab("")]
        public string EmptyString { get; set; }

        [Tab("")]
        public string EmptyString2 { get; set; }

        [Tab(null)]
        public string NullString { get; set; }

        [Tab(null)]
        public string NullString2 { get; set; }
    }

    [Theory]
    [InlineData("NotMapped", "Forms.MyForm.Tabs.A")]
    [InlineData("FormOnly", "Forms.MyForm.Tabs.B")]
    [InlineData("FormOnlyTextKey", "Site.FormOnlyTextKey")]
    [InlineData("FormOverride", "Forms.MyForm.Tabs.C")]
    [InlineData("FormOverrideTextKey", "Site.FormCustomTextKeyOverride")]
    [InlineData("RowOnly", "Db.MyRow.Tabs.R")]
    [InlineData("RowOnlyTextKey", "Site.CustomRowOnlyTextKey")]
    [InlineData("EmptyString", "")]
    [InlineData("EmptyString2", "")]
    [InlineData("NullString", null)]
    [InlineData("NullString2", null)]
    public void Tab_Should_Use_Row_Properties_If_Available(string propertyName,
        string key)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(TabWithRowForm).GetProperty(propertyName);
        var source = new PropertyInfoSource(property,
            new TabWithRowRow());

        processor.Process(source, item);

        Assert.Equal(key, item.Tab);
    }
}