namespace Serenity.Demo.Northwind.Forms;

[ColumnsScript("Northwind.Region")]
[BasedOnRow(typeof(RegionRow), CheckNames = true)]
public class RegionColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public int RegionID { get; set; }
    [EditLink, Width(300)]
    public string RegionDescription { get; set; }
}