using Serenity.Reporting;

namespace Serenity.Demo.Northwind.Columns;

[ColumnsScript("Northwind.Customer")]
[BasedOnRow(typeof(CustomerRow), CheckNames = true)]
public class CustomerColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), Width(100)]
    public string CustomerID { get; set; }
    [EditLink, Width(250)]
    public string CompanyName { get; set; }
    [Width(150)]
    public string ContactName { get; set; }
    [Width(150)]
    public string ContactTitle { get; set; }
    [Width(60)]
    public string Region { get; set; }
    [Width(100)]
    public string PostalCode { get; set; }
    [Width(130), AsyncLookupEditor(typeof(Lookups.CustomerCountryLookup)), QuickFilter(CssClass = "hidden-xs")]
    public string Country { get; set; }
    [Width(120), AsyncLookupEditor(typeof(Lookups.CustomerCityLookup))]
    [QuickFilter(CssClass = "hidden-xs"), QuickFilterOption("cascadeFrom", "Country")]
    public string City { get; set; }
    [Width(120)]
    public string Phone { get; set; }
    [Width(120)]
    public string Fax { get; set; }
    [Width(250), EmployeeListFormatter, CellDecorator(typeof(EmployeeListDecorator))]
    public string Representatives { get; set; }
}