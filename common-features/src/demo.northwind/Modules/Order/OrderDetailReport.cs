using Serenity.Reporting;

namespace Serenity.Demo.Northwind;

[Report("Northwind.OrderDetail")]
[ReportDesign(MVC.Views.Order.OrderDetailReport)]
[RequiredPermission(PermissionKeys.General)]
public class OrderDetailReport(ISqlConnections sqlConnections) : IReport, ICustomizeHtmlToPdf
{
    public int OrderID { get; set; }

    protected ISqlConnections SqlConnections { get; } = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));

    public object GetData()
    {
        var data = new OrderDetailReportData();

        using (var connection = SqlConnections.NewFor<OrderRow>())
        {
            var o = OrderRow.Fields;

            data.Order = connection.TryById<OrderRow>(OrderID, q => q
                 .SelectTableFields()
                 .Select(o.EmployeeFullName)
                 .Select(o.ShipViaCompanyName)) ?? new OrderRow();

            var od = OrderDetailRow.Fields;
            data.Details = connection.List<OrderDetailRow>(q => q
                .SelectTableFields()
                .Select(od.ProductName)
                .Select(od.LineTotal)
                .Where(od.OrderID == OrderID));

            var c = CustomerRow.Fields;
            data.Customer = connection.TryFirst<CustomerRow>(c.CustomerID == data.Order.CustomerID)
                ?? new CustomerRow();
        }

        return data;
    }

    public void Customize(IHtmlToPdfOptions options)
    {
        // you may customize HTML to PDF converter (WKHTML) parameters here, e.g. 
        // options.MarginsAll = "2cm";
    }
}

public class OrderDetailReportData
{
    public OrderRow Order { get; set; }
    public List<OrderDetailRow> Details { get; set; }
    public CustomerRow Customer { get; set; }
}