namespace Serenity.Demo.Northwind.Forms;

[FormScript(InferKey = true)]
[BasedOnRow(typeof(RegionRow), CheckNames = true)]
public class RegionForm
{
    public int? RegionID { get; set; }
    public string? RegionDescription { get; set; }
}