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
    public String CustomerID { get; set; }

    [Category("Customer Details")]
    [ReadOnly(true)]
    public String CustomerContactName { get; set; }
    [ReadOnly(true)]
    public String CustomerContactTitle { get; set; }
    [ReadOnly(true)]
    public String CustomerCity { get; set; }
    [ReadOnly(true)]
    public String CustomerRegion { get; set; }
    [ReadOnly(true)]
    public String CustomerCountry { get; set; }
    [ReadOnly(true)]
    public String CustomerPhone { get; set; }
    [ReadOnly(true)]
    public String CustomerFax { get; set; }

    [Category("Order Details")]
    [DefaultValue("now")]
    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public Int32? EmployeeID { get; set; }
    [OrderDetailsEditor]
    public List<OrderDetailRow> DetailList { get; set; } 
}