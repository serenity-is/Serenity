namespace Serenity.Reporting;

/// <summary>
/// The columns of a report get the attributes of the members of the interfaces their properties implement, as their property
/// items do.
/// </summary>
public class ReportColumnConverterInterfaceTests
{
    public class TestDecorator : BaseCellDecorator
    {
        public override void Decorate()
        {
        }
    }

    public interface IReportGroup
    {
        [DisplayName("Total Amount"), Size(120), DisplayFormat("#,##0.00"), CellDecorator(typeof(TestDecorator))]
        decimal? Total { get; set; }

        [IgnoreUIField]
        string? Hidden { get; set; }
    }

    private class Columns : IReportGroup
    {
        public string? Name { get; set; }

        [DisplayName("Own Title")]
        public decimal? Total { get; set; }

        public string? Hidden { get; set; }
    }

    [Fact]
    public void Column_Has_The_Attributes_Of_The_Interface_Member_With_Its_Own_Taking_Precedence()
    {
        var column = ReportColumnConverter.FromPropertyInfo(typeof(Columns).GetProperty(nameof(Columns.Total)), NullTextLocalizer.Instance);

        Assert.Equal("Own Title", column.Title);
        Assert.Equal(120, column.Width);
        Assert.Equal("#,##0.00", column.Format);
    }

    [Fact]
    public void Columns_Leave_Out_An_Ignored_Interface_Member_And_Get_Its_Decorator()
    {
        var columns = ReportColumnConverter.ObjectTypeToList(typeof(Columns), new ServiceCollection().BuildServiceProvider(),
            NullTextLocalizer.Instance);

        Assert.Equal(["Name", "Total"], columns.Select(x => x.Name));
        Assert.IsType<TestDecorator>(columns.Single(x => x.Name == "Total").Decorator);
    }
}
