using System.Collections;
using ClosedXML.Excel;

namespace Serenity.Reporting;

public class ExcelReportGeneratorTests
{
    private class TestDecorator : ICellDecorator
    {
        public object? Item { get; set; }
        public string? Name { get; set; }
        public object? Value { get; set; }
        public string? Background { get; set; }
        public string? Foreground { get; set; }
        public string? Format { get; set; }
        public string? FormatOverride { get; set; }
        public int DecorateCalls { get; private set; }

        public void Decorate()
        {
            DecorateCalls++;
            Value = "decorated";
            Background = "#FF0000";
            Foreground = "#00FF00";
            Format = FormatOverride ?? "0.00";
        }
    }

    private static List<ReportColumn> Columns(params string[] names)
    {
        return [.. names.Select(x => new ReportColumn { Name = x })];
    }

    [Fact]
    public void GeneratePackageBytes_Returns_Bytes_For_Row_Data()
    {
        var rows = new List<object>
        {
            new MockUserRow { UserId = 1, Name = "A", Email = "a@b.c" }
        };

        var bytes = ExcelReportGenerator.GeneratePackageBytes(
            Columns("Name", "Email"), rows);

        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void GeneratePackage_Creates_Workbook_With_Sheet()
    {
        using var workbook = ExcelReportGenerator.GeneratePackage(
            Columns("Name"), new List<object> { new MockUserRow { Name = "A" } }, "MySheet");

        Assert.Contains("MySheet", workbook.Worksheets.Select(x => x.Name));
    }

    [Fact]
    public void PopulateSheet_Throws_For_Null_Arguments()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        Assert.Throws<ArgumentNullException>(() =>
            ExcelReportGenerator.PopulateSheet(sheet, null!, new ArrayList()));
        Assert.Throws<ArgumentNullException>(() =>
            ExcelReportGenerator.PopulateSheet(sheet, Columns("A"), null!));
    }

    [Fact]
    public void PopulateSheet_Handles_Dictionary_String_Object()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var rows = new List<object>
        {
            new Dictionary<string, object> { ["Name"] = "A" }
        };

        ExcelReportGenerator.PopulateSheet(sheet, Columns("Name"), rows);

        Assert.Equal("A", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Handles_NonGeneric_Dictionary()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var rows = new List<object> { new Hashtable { ["Name"] = "B" } };

        ExcelReportGenerator.PopulateSheet(sheet, Columns("Name"), rows);

        Assert.Equal("B", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Handles_Plain_Objects()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var rows = new List<object> { new { Name = "C" } };

        ExcelReportGenerator.PopulateSheet(sheet, Columns("Name"), rows);

        Assert.Equal("C", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Ignores_Invalid_Properties()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var rows = new List<object> { new { Name = "C" } };

        ExcelReportGenerator.PopulateSheet(sheet, Columns("Name", "Missing"), rows);

        Assert.Equal("C", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Applies_Column_Format()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn>
        {
            new() { Name = "Name", Format = "0.00", DataType = typeof(double) }
        };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new { Name = "C" } });

        Assert.Equal("0.00", sheet.Column(1).Style.NumberFormat.Format);
    }

    [Fact]
    public void PopulateSheet_Fixes_DateTime_Format()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn>
        {
            new() { Name = "Name", Format = "yyyy-MM-dd f", DataType = typeof(DateTime) }
        };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new { Name = "C" } });

        Assert.Equal("yyyy-MM-dd 0", sheet.Column(1).Style.NumberFormat.Format);
    }

    [Fact]
    public void PopulateSheet_Uses_Decorator()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn>
        {
            new() { Name = "Name", Decorator = decorator }
        };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new { Name = "C" } });

        Assert.Equal(1, decorator.DecorateCalls);
        Assert.Equal("decorated", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Creates_Table()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");

        ExcelReportGenerator.PopulateSheet(sheet, Columns("Name"), new List<object> { new { Name = "C" } },
            tableName: "MyTable", tableStyle: XLTableTheme.TableStyleLight9);

        var table = Assert.Single(sheet.Tables);
        Assert.Equal("MyTable", table.Name);
    }

    [Fact]
    public void PopulateSheet_Handles_Null_Column_Name()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn>
        {
            new() { Name = null },
            new() { Name = "Name" }
        };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object>
        {
            new Dictionary<string, object> { ["Name"] = "A" }
        });

        Assert.Equal(" ", sheet.Cell(1, 1).GetString());
    }

    [Fact]
    public void PopulateSheet_Uses_Decorator_For_Row_Data()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Name", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new MockUserRow { Name = "C" } });

        Assert.Equal(1, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Uses_Decorator_For_Dictionary_String_Object()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Name", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object>
        {
            new Dictionary<string, object> { ["Name"] = "C" }
        });

        Assert.Equal(1, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Decorator_Handles_Missing_Dictionary_Key()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Missing", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object>
        {
            new Dictionary<string, object> { ["Name"] = "C" }
        });

        Assert.Equal(1, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Uses_Decorator_For_NonGeneric_Dictionary()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Name", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object>
        {
            new Hashtable { ["Name"] = "C" }
        });

        Assert.Equal(1, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Uses_Decorator_For_Plain_Object()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Name", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new { Name = "C" } });

        Assert.Equal(1, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Skips_Decorator_For_Null_Row()
    {
        var decorator = new TestDecorator();
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn> { new() { Name = "Name", Decorator = decorator } };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object?> { null });

        Assert.Equal(0, decorator.DecorateCalls);
    }

    [Fact]
    public void PopulateSheet_Empty_Decorator_Format_Returns_Unchanged()
    {
        var decorator = new TestDecorator { FormatOverride = "" };
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var columns = new List<ReportColumn>
        {
            new() { Name = "Name", Decorator = decorator }
        };

        ExcelReportGenerator.PopulateSheet(sheet, columns, new List<object> { new { Name = "C" } });
    }

    [Fact]
    public void AsObject_Converts_Cell_Values()
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("S");
        var date = new DateTime(2024, 1, 1);
        var span = TimeSpan.FromMinutes(5);

        sheet.Cell(1, 1).Value = true;
        sheet.Cell(2, 1).Value = 1.5;
        sheet.Cell(3, 1).Value = "text";
        sheet.Cell(4, 1).Value = date;
        sheet.Cell(5, 1).Value = span;
        sheet.Cell(6, 1).Value = XLError.DivisionByZero;

        Assert.True((bool)sheet.Cell(1, 1).Value.AsObject()!);
        Assert.Equal(1.5, sheet.Cell(2, 1).Value.AsObject());
        Assert.Equal("text", sheet.Cell(3, 1).Value.AsObject());
        Assert.Equal(date, sheet.Cell(4, 1).Value.AsObject());
        Assert.Equal(span, sheet.Cell(5, 1).Value.AsObject());
        Assert.Equal(XLError.DivisionByZero, sheet.Cell(6, 1).Value.AsObject());
        Assert.Null(sheet.Cell(7, 1).Value.AsObject());
    }
}
