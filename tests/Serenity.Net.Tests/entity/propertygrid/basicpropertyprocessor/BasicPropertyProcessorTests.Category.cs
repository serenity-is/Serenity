using Serenity.PropertyGrid;

namespace Serenity.Tests.Entity;

public partial class BasicPropertyProcessorTests
{
    private class CategoryWithTextKeys
    {
        [Category("Db.SomeEntity.SomeField")]
        public string Db { get; set; }

        [Category("Columns.SomeColumns.SomeFieldCategory")]
        public string Cols { get; set; }

        [Category("Forms.SomeForm.SomeKeyCategory")]
        public string Forms { get; set; }

        [Category("Site.SomeKey")]
        public string Site { get; set; }
    }

    [Theory]
    [InlineData(nameof(CategoryWithTextKeys.Db), "Db.SomeEntity.SomeField")]
    [InlineData(nameof(CategoryWithTextKeys.Cols), "Columns.SomeColumns.SomeFieldCategory")]
    [InlineData(nameof(CategoryWithTextKeys.Forms), "Forms.SomeForm.SomeKeyCategory")]
    [InlineData(nameof(CategoryWithTextKeys.Site), "Site.SomeKey")]
    public void Category_Should_Use_CategoryAttribute_With_LocalTextKey(string propertyName, string expectedKey)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var source = new PropertyInfoSource(typeof(CategoryWithTextKeys)
            .GetProperty(propertyName), null);

        processor.Process(source, item);

        Assert.Equal(expectedKey, item.Category);
    }

    [ColumnsScript]
    private class CategoryWithNonSupportedKeyLike
    {
        [Category("S.No")]
        public string SNo { get; set; }

        [Category("Inv.Number")]
        public string Forms { get; set; }

        [Category("DB.ID")]
        public string DBID { get; set; }
    }

    [Theory]
    [InlineData(nameof(CategoryWithNonSupportedKeyLike.SNo), "S.No")]
    [InlineData(nameof(CategoryWithNonSupportedKeyLike.Forms), "Inv.Number")]
    [InlineData(nameof(CategoryWithNonSupportedKeyLike.DBID), "DB.ID")]
    public void Category_Should_Consider_Category_With_Unsupported_Prefixes_As_Text(
        string propertyName, string category)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(CategoryWithNonSupportedKeyLike).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal($"Columns.{typeof(CategoryWithNonSupportedKeyLike).FullName}.Categories.{category}",
            item.Category);
    }

    private class CategoryWithNonSupportedKeyLikeNoAttr
    {
        [Category("S.No")]
        public string SNo { get; set; }
    }

    [Fact]
    public void Category_Should_Use_TextsAsIs_For_Types_Without_ColumnScript_And_FormScript()
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var propertyName = nameof(CategoryWithNonSupportedKeyLikeNoAttr.SNo);
        var property = typeof(CategoryWithNonSupportedKeyLikeNoAttr)
            .GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        // display name is not possibly registered as local text so uses the text as is
        Assert.Equal("S.No", item.Category);
    }

    [FormScript]
    private class CategoryWithRegularText
    {
        [Category("Starting Date")]
        public string StartDate { get; set; }

        [Category("ID")]
        public string SomeID { get; set; }

        [Category("")]
        public string Empty { get; set; }

        [Category("Date of Birth")]
        public string DOB { get; set; }

        [Category(null)]
        public string Null { get; set; }
    }

    [Theory]
    [InlineData(nameof(CategoryWithRegularText.StartDate), "Starting Date")]
    [InlineData(nameof(CategoryWithRegularText.SomeID), "ID")]
    [InlineData(nameof(CategoryWithRegularText.DOB), "Date of Birth")]
    [InlineData(nameof(CategoryWithRegularText.Empty), "")]
    [InlineData(nameof(CategoryWithRegularText.Null), null)]
    public void Category_Should_Use_CategoryAttribute_With_Regular_Text_AsIs(
        string propertyName, string category)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(CategoryWithRegularText).GetProperty(propertyName);
        var source = new PropertyInfoSource(property, null);

        processor.Process(source, item);

        Assert.Equal(string.IsNullOrEmpty(category) ? category : 
            $"Forms.{typeof(CategoryWithRegularText).FullName}.Categories.{category}",
            item.Category);
    }

    [LocalTextPrefix("MyRow")]
    public class CategoryWithRowRow : Row<CategoryWithRowRow.RowFields>
    {
        public string FormOnly { get; set; }

        public string FormOnlyTextKey { get; set; }

        [Category("O")]
        public string FormOverride { get; set; }

        [Category("Site.CustomRowTextKey")]
        public string FormOverrideTextKey { get; set; }

        [Category("R")]
        public string RowOnly { get; set; }

        [Category("Site.CustomRowOnlyTextKey")]
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

    [FormScript("MyForm"), BasedOnRow(typeof(CategoryWithRowRow))]
    private class CategoryWithRowForm
    {
        [Category("A")]
        public string NotMapped { get; set; }

        [Category("B")]
        public string FormOnly { get; set; }

        [Category("Site.FormOnlyTextKey")]
        public string FormOnlyTextKey { get; set; }

        [Category("C")]
        public string FormOverride { get; set; }

        [Category("Site.FormCustomTextKeyOverride")]
        public string FormOverrideTextKey { get; set; }

        public string RowOnly { get; set; }

        public string RowOnlyTextKey { get; set; }

        [Category("Category Before")]
        public string CategoryBefore { get; set; }
        [Category("")]
        public string CategoryEmpty { get; set; }
    }

    [Theory]
    [InlineData("NotMapped", "Forms.MyForm.Categories.A")]
    [InlineData("FormOnly", "Forms.MyForm.Categories.B")]
    [InlineData("FormOnlyTextKey", "Site.FormOnlyTextKey")]
    [InlineData("FormOverride", "Forms.MyForm.Categories.C")]
    [InlineData("FormOverrideTextKey", "Site.FormCustomTextKeyOverride")]
    [InlineData("RowOnly", "Db.MyRow.Categories.R")]
    [InlineData("RowOnlyTextKey", "Site.CustomRowOnlyTextKey")]
    [InlineData("CategoryBefore", "Forms.MyForm.Categories.Category Before")]
    [InlineData("CategoryEmpty", "")]
    public void Category_Should_Use_Row_Properties_If_Available(string propertyName,
        string key)
    {
        var processor = new BasicPropertyProcessor();

        var item = new PropertyItem();
        var property = typeof(CategoryWithRowForm).GetProperty(propertyName);
        var source = new PropertyInfoSource(property,
            new CategoryWithRowRow());

        processor.Process(source, item);

        Assert.Equal(key, item.Category);
    }
}