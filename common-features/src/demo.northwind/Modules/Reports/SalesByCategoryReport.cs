using Serenity.Reporting;

namespace Serenity.Demo.Northwind;

[Report, RequiredPermission(PermissionKeys.General)]
[Category("Northwind/Orders"), DisplayName("Sales By Category")]
public class SalesByDetailReport(ISqlConnections sqlConnections, ITextLocalizer localizer, IServiceProvider serviceProvider) : IReport, IDataOnlyReport
{
    protected ISqlConnections SqlConnections { get; } = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    protected ITextLocalizer Localizer { get; } = localizer ?? throw new ArgumentNullException(nameof(localizer));
    protected IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public object GetData()
    {
        using var connection = SqlConnections.NewFor<SalesByCategoryRow>();
        var s = SalesByCategoryRow.Fields;

        return connection.List<SalesByCategoryRow>();
    }

    public List<ReportColumn> GetColumnList()
    {
        return ReportColumnConverter.ObjectTypeToList(typeof(Item), ServiceProvider, Localizer);
    }

    [BasedOnRow(typeof(SalesByCategoryRow), CheckNames = true)]
    public class Item
    {
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal ProductSales { get; set; }
    }
}