using Serenity.Demo.Northwind;

namespace Serenity.Demo.BasicSamples.Forms;

/// <summary>
/// This is a basic Order form for FilteredLookupInDetailDialog sample.
/// </summary>
[FormScript("BasicSamples.FilteredLookupInDetail")]
[BasedOnRow(typeof(OrderRow), CheckNames = true)]
public class FilteredLookupInDetailForm
{
    public String CustomerID { get; set; }
    [DefaultValue("now")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// We use OneWay here for this sample, because we don't
    /// actually have a CategoryID field in order row.
    /// Otherwise it would be serialized to JSON on save, 
    /// and as we didn't have such a field in OrderRow,
    /// it would raise an error on deserialization.
    /// </summary>
    [DisplayName("Category"), LookupEditor(typeof(CategoryRow)), OneWay, IgnoreName]
    public Int32? CategoryID { get; set; }

    [Category("Order Details")]
    [FilteredLookupDetailEditor]
    public List<OrderDetailRow> DetailList { get; set; }
}