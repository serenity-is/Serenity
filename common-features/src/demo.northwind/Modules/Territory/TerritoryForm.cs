namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Territory")]
[BasedOnRow(typeof(TerritoryRow), CheckNames = true)]
public class TerritoryForm
{
    public string TerritoryID { get; set; }
    public string TerritoryDescription { get; set; }
    [LookupEditor(typeof(RegionRow))]
    public int RegionID { get; set; }
}