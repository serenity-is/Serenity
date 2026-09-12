using Serenity.Demo.Northwind;

namespace Serenity.Demo.BasicSamples.Forms;

/// <summary>
/// A custom order form that contains read-only details about customer
/// </summary>
[FormScript("BasicSamples.PopulateLinkedData")]
[BasedOnRow(typeof(OrderRow), CheckNames = true)]
public class PopulateLinkedDataForm
{
    [Category("Order")]
    public string? CustomerID { get; set; }

    [Category("Customer Details")]
    [ReadOnly(true)]
    public string? CustomerContactName { get; set; }
    [ReadOnly(true)]
    public string? CustomerContactTitle { get; set; }
    [ReadOnly(true)]
    public string? CustomerCity { get; set; }
    [ReadOnly(true)]
    public string? CustomerRegion { get; set; }
    [ReadOnly(true)]
    public string? CustomerCountry { get; set; }
    [ReadOnly(true)]
    public string? CustomerPhone { get; set; }
    [ReadOnly(true)]
    public string? CustomerFax { get; set; }

    [Category("Order Details")]
    [DefaultValue("now")]
    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public int? EmployeeID { get; set; }
    [OrderDetailsEditor]
    public List<OrderDetailRow>? DetailList { get; set; } 
}