using Serenity.PropertyGrid;

namespace Serenity.Reporting;

public class TabularDataReportTests
{
    public enum TestEnum
    {
        A = 1,
        B = 2
    }

    [TableName("TabularReportTest")]
    private class TestRow : Row<TestRow.RowFields>, IRow
    {
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        [Size(50)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public DateTime? Created { get => fields.Created[this]; set => fields.Created[this] = value; }
        public TestEnum? Enum { get => fields.Enum[this]; set => fields.Enum[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public DateTimeField Created;
            public EnumField<TestEnum> Enum;
#pragma warning restore CS0649
        }
    }

    [BasedOnRow(typeof(TestRow))]
    private class TestColumns
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public DateTime? Created { get; set; }
        public TestEnum? Enum { get; set; }

        [CellDecorator(typeof(TestCellDecorator))]
        public string? Decorated { get; set; }
    }

    private class CustomizedColumns : ICustomizePropertyItems
    {
        public string? Name { get; set; }

        public void Customize(List<PropertyItem> input)
        {
            input.Add(new PropertyItem { Name = "Added", Title = "Added Title" });
        }
    }

    private class PlainColumns
    {
        public string? Value { get; set; }
    }

    public class TestCellDecorator : BaseCellDecorator
    {
        public override void Decorate()
        {
        }
    }

    private class TestPropertyItemProvider(IEnumerable<PropertyItem> sourceItems) : IPropertyItemProvider
    {
        private readonly List<PropertyItem> items = [.. sourceItems];

        public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type, Func<PropertyInfo, bool>? predicate = null)
        {
            return items;
        }
    }

    private class TestExportReport : TabularDataReport
    {
        public TestExportReport(IEnumerable<string> exportColumns)
        {
            ExportColumns = exportColumns;
        }
    }

    private static ServiceProvider CreateProvider(IPropertyItemProvider? propertyItemProvider = null,
        ITextLocalizer? localizer = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITwoLevelCache>(new NullTwoLevelCache());
        services.AddSingleton(propertyItemProvider ?? new TestPropertyItemProvider([]));
        services.AddSingleton(localizer ?? new MockTextLocalizer());
        return services.BuildServiceProvider();
    }

    private static List<PropertyItem> DefaultItems()
    {
        return
        [
            new PropertyItem { Name = "ID" },
            new PropertyItem { Name = "Name", Title = "Name Title", Width = 50 },
            new PropertyItem { Name = "Created" },
            new PropertyItem { Name = "Enum" },
            new PropertyItem { Name = "Decorated" }
        ];
    }

    [Fact]
    public void CacheGroupKey_Is_TypeName()
    {
        Assert.Equal(nameof(TabularDataReport), TabularDataReport.CacheGroupKey);
    }

    [Fact]
    public void Constructor_With_Columns_Throws_For_Null_Data()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(null, new[] { new ReportColumn { Name = "A" } }));
    }

    [Fact]
    public void Constructor_With_Columns_Throws_For_Null_Columns()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(new object[] { 1 }, null));
    }

    [Fact]
    public void Constructor_With_Columns_Uses_Provided_Columns()
    {
        var data = new object[] { 1, 2 };
        var report = new TabularDataReport(data, new[] { new ReportColumn { Name = "A" } });

        Assert.Same(data, report.GetData());

        var columns = report.GetColumnList();
        var column = Assert.Single(columns);
        Assert.Equal("A", column.Name);
    }

    [Fact]
    public void Constructor_With_ColumnsType_Throws_For_Nulls()
    {
        var data = new object[] { 1 };
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(null, typeof(TestColumns), CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(data, null, CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(data, typeof(TestColumns), null));
    }

    [Fact]
    public void Constructor_With_ColumnsType_Builds_Columns()
    {
        var provider = CreateProvider(new TestPropertyItemProvider(DefaultItems()));
        var report = new TabularDataReport(new object[] { 1 }, typeof(TestColumns), provider);

        var columns = report.GetColumnList();
        Assert.Equal(5, columns.Count);
    }

    [Fact]
    public void Constructor_With_ExportColumns_Throws_For_Nulls()
    {
        var data = new object[] { 1 };
        var exportColumns = new[] { "Name" };
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(null, typeof(TestColumns), exportColumns, CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(data, null, exportColumns, CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(data, typeof(TestColumns), null, CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            new TabularDataReport(data, typeof(TestColumns), exportColumns, null));
    }

    [Fact]
    public void Constructor_With_ExportColumns_Subsets_Columns()
    {
        var provider = CreateProvider(new TestPropertyItemProvider(DefaultItems()));
        var report = new TabularDataReport(new object[] { 1 }, typeof(TestColumns), ["Name"], provider);

        var column = Assert.Single(report.GetColumnList());
        Assert.Equal("Name", column.Name);
    }

    [Fact]
    public void GetColumnList_Without_ColumnsType_Uses_ExportColumn_Names()
    {
        var report = new TestExportReport(["X", "Y"]);

        var columns = report.GetColumnList();
        Assert.Equal(2, columns.Count);
        Assert.Equal("X", columns[0].Name);
        Assert.Equal("Y", columns[1].Name);
        Assert.Null(columns[0].DataType);
    }

    [Fact]
    public void GetColumnListFor_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TabularDataReport.GetColumnListFor(null, null, CreateProvider()));
        Assert.Throws<ArgumentNullException>(() =>
            TabularDataReport.GetColumnListFor(typeof(TestColumns), null, null));
    }

    [Fact]
    public void GetColumnListFor_Returns_Empty_For_Empty_ExportColumns()
    {
        var provider = CreateProvider(new TestPropertyItemProvider(DefaultItems()));
        var columns = TabularDataReport.GetColumnListFor(typeof(TestColumns), [], provider);
        Assert.Empty(columns);
    }

    [Fact]
    public void GetColumnListFor_Skips_Missing_Columns()
    {
        var provider = CreateProvider(new TestPropertyItemProvider(DefaultItems()));
        var columns = TabularDataReport.GetColumnListFor(typeof(TestColumns), ["Name", "Missing"], provider);

        var column = Assert.Single(columns);
        Assert.Equal("Name", column.Name);
    }

    [Fact]
    public void GetColumnListFor_Uses_BasedOnRow_Field()
    {
        var provider = CreateProvider(new TestPropertyItemProvider([
            new PropertyItem { Name = "Name" },
            new PropertyItem { Name = "Decorated" }
        ]));
        var columns = TabularDataReport.GetColumnListFor(typeof(TestColumns), null, provider);

        Assert.Equal(2, columns.Count);
        Assert.Equal("Name", columns[0].Name);
        Assert.IsType<TestCellDecorator>(columns[1].Decorator);
    }

    [Fact]
    public void GetColumnListFor_Customizes_PropertyItems()
    {
        var provider = CreateProvider(new TestPropertyItemProvider([
            new PropertyItem { Name = "Name", Title = "N" }
        ]));
        var columns = TabularDataReport.GetColumnListFor(typeof(CustomizedColumns), null, provider);

        Assert.Equal(2, columns.Count);
        Assert.Equal("N", columns[0].Title);
        Assert.Equal("Added", columns[1].Name);
        Assert.Equal("Added Title", columns[1].Title);
    }

    [Fact]
    public void GetColumnListFor_Without_BasedOnRow_Leaves_Field_Null()
    {
        var provider = CreateProvider(new TestPropertyItemProvider([
            new PropertyItem { Name = "Value" }
        ]));
        var columns = TabularDataReport.GetColumnListFor(typeof(PlainColumns), null, provider);

        var column = Assert.Single(columns);
        Assert.Equal("Value", column.Name);
        Assert.Null(column.DataType);
    }

    [Fact]
    public void FromPropertyItem_Throws_For_Nulls()
    {
        var provider = CreateProvider();
        Assert.Throws<ArgumentNullException>(() =>
            TabularDataReport.FromPropertyItem(null, null, null, provider, NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            TabularDataReport.FromPropertyItem(new PropertyItem { Name = "X" }, null, null, provider, null));
    }

    [Fact]
    public void FromPropertyItem_Localizes_Title_And_Copies_Width()
    {
        var provider = CreateProvider();
        var localizer = new MockTextLocalizer(x => "LOC:" + x);
        var item = new PropertyItem { Name = "Name", Title = "Name Title", Width = 20 };

        var column = TabularDataReport.FromPropertyItem(item, null, null, provider, localizer);

        Assert.Equal("Name", column.Name);
        Assert.Equal("LOC:Name Title", column.Title);
        Assert.Equal(20, column.Width);
    }

    [Fact]
    public void FromPropertyItem_Uses_StringField_Size_For_Width()
    {
        var provider = CreateProvider();
        var row = new TestRow();
        var item = new PropertyItem { Name = "Name" };

        var column = TabularDataReport.FromPropertyItem(item, row.GetFields().Name, null, provider,
            NullTextLocalizer.Instance);

        Assert.Equal(50, column.Width);
        Assert.Equal(typeof(string), column.DataType);
    }

    [Theory]
    [InlineData("d", "Date")]
    [InlineData("g", "Date")]
    [InlineData("G", "Date")]
    [InlineData("s", "Date")]
    [InlineData("u", "Date")]
    [InlineData("yyyy", "DateTime")]
    public void FromPropertyItem_Formats_Date_DisplayFormat(string format, string formatterType)
    {
        var provider = CreateProvider();
        var item = new PropertyItem { Name = "Created", DisplayFormat = format, FormatterType = formatterType };

        var column = TabularDataReport.FromPropertyItem(item, null, null, provider, NullTextLocalizer.Instance);

        Assert.NotNull(column.Format);
    }

    [Fact]
    public void FromPropertyItem_Uses_DisplayFormat_For_NonDate()
    {
        var provider = CreateProvider();
        var item = new PropertyItem { Name = "Amount", DisplayFormat = "#,##0.00" };

        var column = TabularDataReport.FromPropertyItem(item, null, null, provider, NullTextLocalizer.Instance);

        Assert.Equal("#,##0.00", column.Format);
    }

    [Fact]
    public void FromPropertyItem_Uses_DateTimeFormat_For_Utc_Field()
    {
        var provider = CreateProvider();
        var row = new TestRow();
        var fields = row.GetFields();
        fields.Created.DateTimeKind = DateTimeKind.Utc;
        var item = new PropertyItem { Name = "Created", DisplayFormat = " " };

        var column = TabularDataReport.FromPropertyItem(item, fields.Created, null, provider,
            NullTextLocalizer.Instance);

        Assert.Equal(DateHelper.CurrentDateTimeFormat, column.Format);
    }

    [Fact]
    public void FromPropertyItem_Uses_DateFormat_For_Unspecified_Field()
    {
        var provider = CreateProvider();
        var row = new TestRow();
        var fields = row.GetFields();
        fields.Created.DateTimeKind = DateTimeKind.Unspecified;
        var item = new PropertyItem { Name = "Created", DisplayFormat = " " };

        var column = TabularDataReport.FromPropertyItem(item, fields.Created, null, provider,
            NullTextLocalizer.Instance);

        Assert.Equal(DateHelper.CurrentDateFormat, column.Format);
    }

    [Fact]
    public void FromPropertyItem_Adds_Enum_Decorator()
    {
        var provider = CreateProvider();
        var row = new TestRow();
        var item = new PropertyItem { Name = "Enum" };

        var column = TabularDataReport.FromPropertyItem(item, row.GetFields().Enum, null, provider,
            NullTextLocalizer.Instance);

        Assert.IsType<EnumDecorator>(column.Decorator);
    }

    [Fact]
    public void FromPropertyItem_Adds_Cell_Decorator_From_Property()
    {
        var provider = CreateProvider();
        var property = typeof(TestColumns).GetProperty(nameof(TestColumns.Decorated));
        var item = new PropertyItem { Name = "Decorated" };

        var column = TabularDataReport.FromPropertyItem(item, null, property, provider,
            NullTextLocalizer.Instance);

        Assert.IsType<TestCellDecorator>(column.Decorator);
    }

    [Fact]
    public void FromPropertyItem_Uses_Field_Title_When_Item_Title_Missing()
    {
        var provider = CreateProvider();
        var row = new TestRow();
        var item = new PropertyItem { Name = "Created" };

        var column = TabularDataReport.FromPropertyItem(item, row.GetFields().Created, null, provider,
            NullTextLocalizer.Instance);

        Assert.NotNull(column.Title);
        Assert.Null(column.Width);
    }
}
