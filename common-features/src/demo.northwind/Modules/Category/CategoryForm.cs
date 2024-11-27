namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Category")]
[BasedOnRow(typeof(CategoryRow), CheckNames = true)]
public class CategoryForm
{
    public string CategoryName { get; set; }
    public string Description { get; set; }
}