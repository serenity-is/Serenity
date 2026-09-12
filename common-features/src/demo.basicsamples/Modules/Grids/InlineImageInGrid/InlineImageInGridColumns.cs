namespace Serenity.Demo.BasicSamples.Columns;

[ColumnsScript("BasicSamples.InlineImageInGrid")]
[BasedOnRow(typeof(Northwind.ProductRow), CheckNames = true)]
public class InlineImageInGridColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public string? ProductID { get; set; }
    [EditLink, Width(250)]
    public string? ProductName { get; set; }
    [InlineImageFormatter, Width(450)]
    public string? ProductImage { get; set; }
    [NotMapped, InlineImageFormatter(FileProperty = "ProductImage", Thumb = true), Width(450)]
    public string? ProductThumbnail { get; set; }
}