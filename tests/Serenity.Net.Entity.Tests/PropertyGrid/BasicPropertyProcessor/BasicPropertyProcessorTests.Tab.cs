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
}