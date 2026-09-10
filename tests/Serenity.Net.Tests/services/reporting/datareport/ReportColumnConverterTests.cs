namespace Serenity.Reporting;

public class ReportColumnConverterTests
{
    [TableName("ReportColumnTest")]
    private class SourceRow : Row<SourceRow.RowFields>, IRow
    {
        [DisplayName("Row Name"), Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public DateTime? Created { get => fields.Created[this]; set => fields.Created[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public StringField Name;
            public DateTimeField Created;
#pragma warning restore CS0649
        }
    }

    public class TestDecorator : BaseCellDecorator
    {
        public override void Decorate()
        {
        }
    }

    [BasedOnRow(typeof(SourceRow))]
    private class TestColumns
    {
        public string? Name { get; set; }

        [DisplayFormat("yyyy-MM-dd")]
        public DateTime? Created { get; set; }

        public DateTime? PlainDate { get; set; }

        [Size(77)]
        public int? WidthFromSize { get; set; }

        [IgnoreUIField]
        public string? Ignored { get; set; }

        [TransformIgnore]
        public string? Transformed { get; set; }

        [CellDecorator(typeof(TestDecorator))]
        public int? Status { get; set; }

        [DisplayName("Display Title")]
        public string? WithDisplay { get; set; }

#pragma warning disable CS0649
        public string? ExtraField;
#pragma warning restore CS0649
    }

    private class NoBaseColumns
    {
        public string? Value { get; set; }
    }

    private static ServiceProvider CreateServices()
    {
        return new ServiceCollection().BuildServiceProvider();
    }

    [Fact]
    public void FromFieldInfo_Throws_For_Null_Field()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ReportColumnConverter.FromFieldInfo(null, NullTextLocalizer.Instance));
    }

    [Fact]
    public void FromPropertyInfo_Throws_For_Null_Property()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ReportColumnConverter.FromPropertyInfo(null, NullTextLocalizer.Instance));
    }

    [Fact]
    public void FromPropertyInfo_Uses_DisplayFormat()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.Created));
        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance);

        Assert.Equal("Created", column.Name);
        Assert.Equal("yyyy-MM-dd", column.Format);
    }

    [Fact]
    public void FromPropertyInfo_Uses_Size_Attribute()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.WidthFromSize));
        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance);

        Assert.Equal(77, column.Width);
    }

    [Fact]
    public void FromPropertyInfo_Formats_DateTime_DataType()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.PlainDate));
        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance);

        Assert.Equal("dd/MM/yyyy", column.Format);
        Assert.Equal(typeof(DateTime?), column.DataType);
    }

    [Fact]
    public void FromPropertyInfo_DateTimeField_Without_DateOnly_Uses_DateTime_Format()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.PlainDate));
        var baseField = new SourceRow().GetFields().Created;
        baseField.DateOnly = false;

        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance, baseField);

        Assert.Equal("dd/MM/yyyy HH:mm", column.Format);
    }

    [Fact]
    public void FromPropertyInfo_DateTimeField_DateOnly_Uses_Date_Format()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.PlainDate));
        var baseField = new SourceRow().GetFields().Created;
        baseField.DateOnly = true;

        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance, baseField);

        Assert.Equal("dd/MM/yyyy", column.Format);
    }

    [Fact]
    public void FromPropertyInfo_Uses_BaseField_Title_And_Width()
    {
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.Name));
        var baseField = new SourceRow().GetFields().Name;

        var column = ReportColumnConverter.FromPropertyInfo(property, NullTextLocalizer.Instance, baseField);

        Assert.Equal("Row Name", column.Title);
        Assert.Equal(50, column.Width);
    }

    [Fact]
    public void FromFieldInfo_Extracts_Field()
    {
        var fieldInfo = typeof(TestColumns).GetField(nameof(TestColumns.ExtraField));
        var column = ReportColumnConverter.FromFieldInfo(fieldInfo, NullTextLocalizer.Instance);

        Assert.Equal("ExtraField", column.Name);
        Assert.Equal(typeof(string), column.DataType);
    }

    [Fact]
    public void ObjectTypeToList_Extracts_Columns_And_Skips_Ignored()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(TestColumns), CreateServices(), NullTextLocalizer.Instance);

        Assert.Contains(columns, c => c.Name == "Name");
        Assert.Contains(columns, c => c.Name == "Created");
        Assert.Contains(columns, c => c.Name == "PlainDate");
        Assert.Contains(columns, c => c.Name == "WidthFromSize");
        Assert.Contains(columns, c => c.Name == "Status");
        Assert.Contains(columns, c => c.Name == "ExtraField");
        Assert.DoesNotContain(columns, c => c.Name == "Ignored");
        Assert.DoesNotContain(columns, c => c.Name == "Transformed");
    }

    [Fact]
    public void ObjectTypeToList_Creates_CellDecorator()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(TestColumns), CreateServices(), NullTextLocalizer.Instance);

        var status = columns.Single(c => c.Name == "Status");
        Assert.IsType<TestDecorator>(status.Decorator);
    }

    [Fact]
    public void ObjectTypeToList_Uses_BasedOnRow_BaseField()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(TestColumns), CreateServices(), NullTextLocalizer.Instance);

        var name = columns.Single(c => c.Name == "Name");
        Assert.Equal("Row Name", name.Title);
        Assert.Equal(50, name.Width);
    }

    [Fact]
    public void ObjectTypeToList_Uses_Member_DisplayName()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(TestColumns), CreateServices(), NullTextLocalizer.Instance);

        var withDisplay = columns.Single(c => c.Name == "WithDisplay");
        Assert.Equal("Display Title", withDisplay.Title);
    }

    [Fact]
    public void ObjectTypeToList_Without_BasedOnRow_Has_No_BaseField()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(NoBaseColumns), CreateServices(), NullTextLocalizer.Instance);

        var value = Assert.Single(columns);
        Assert.Equal("Value", value.Name);
        Assert.Null(value.Title);
    }

    [Fact]
    public void FromField_Extracts_StringField_Width_And_Title()
    {
        var baseField = new SourceRow().GetFields().Name;
        var column = ReportColumnConverter.FromField(baseField, NullTextLocalizer.Instance);

        Assert.Equal("Name", column.Name);
        Assert.Equal("Row Name", column.Title);
        Assert.Equal(50, column.Width);
    }

    [Fact]
    public void FromField_NonStringField_Has_No_Width()
    {
        var baseField = new SourceRow().GetFields().Created;
        var column = ReportColumnConverter.FromField(baseField, NullTextLocalizer.Instance);

        Assert.Equal("Created", column.Name);
        Assert.Null(column.Width);
    }

    [Fact]
    public void EntityTypeToList_Extracts_All_Fields()
    {
        var row = new SourceRow();
        var columns = ReportColumnConverter.EntityTypeToList(row, NullTextLocalizer.Instance);

        Assert.Equal(2, columns.Count);
        Assert.Contains(columns, c => c.Name == "Name");
        Assert.Contains(columns, c => c.Name == "Created");
    }
}
