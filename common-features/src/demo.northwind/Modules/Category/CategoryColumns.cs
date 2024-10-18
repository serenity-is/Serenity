namespace Serenity.Demo.Northwind.Forms;

[ColumnsScript("Northwind.Category")]
[BasedOnRow(typeof(CategoryRow), CheckNames = true)]
public class CategoryColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public int CategoryID { get; set; }
    [EditLink, Width(250)]
    public string CategoryName { get; set; }
    [Width(450)]
    public string Description { get; set; }
}