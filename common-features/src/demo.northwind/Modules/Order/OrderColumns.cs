namespace Serenity.Demo.Northwind.Columns;

[ColumnsScript("Northwind.Order")]
[BasedOnRow(typeof(OrderRow), CheckNames = true)]
public class OrderColumns
{
    [EditLink, AlignRight, SortOrder(1, descending: true), Width(70), PinToStart]
    public string OrderID { get; set; }

    [DisplayName(""), AlignCenter, Unbound, FixedWidth(15), PinToStart]
    public string PrintInvoice { get; set; }

    [EditLink, Width(200), QuickFilter, PinToStart]
    public string CustomerCompanyName { get; set; }

    [EditLink, QuickFilter(CssClass = "hidden-xs")]
    public DateTime? OrderDate { get; set; }

    [Width(140), EmployeeFormatter(GenderProperty = "EmployeeGender"), QuickFilter(CssClass = "hidden-xs")]
    public string EmployeeFullName { get; set; }

    public DateTime? RequiredDate { get; set; }

    [FilterOnly, QuickFilter]
    public OrderShippingState ShippingState { get; set; }

    public DateTime? ShippedDate { get; set; }

    [Width(140), ShipperFormatter, QuickFilter(CssClass = "hidden-xs"), QuickFilterOption("multiple", true)]
    public string ShipViaCompanyName { get; set; }

    [Width(100), QuickFilter(CssClass = "hidden-xs"), AsyncLookupEditor(typeof(Lookups.OrderShipCountryLookup)), CountryWithFlagFormatter]
    public string ShipCountry { get; set; }

    [Width(100), AsyncLookupEditor(typeof(Lookups.OrderShipCityLookup))]
    [QuickFilter(CssClass = "hidden-xs"), QuickFilterOption("CascadeFrom", "ShipCountry")]
    public string ShipCity { get; set; }

    [FreightFormatter]
    public decimal? Freight { get; set; }
}