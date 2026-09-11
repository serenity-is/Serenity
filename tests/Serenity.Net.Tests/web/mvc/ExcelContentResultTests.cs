namespace Serenity.Web;

public class ExcelContentResultTests
{
    [Fact]
    public void Create_Returns_Excel_Content_Result_With_Default_Download_Name()
    {
        var data = new byte[] { 1, 2, 3 };

        var result = ExcelContentResult.Create(data);

        Assert.Equal(data, result.FileContents);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        Assert.StartsWith("report", result.FileDownloadName);
        Assert.EndsWith(".xlsx", result.FileDownloadName);
    }

    [Fact]
    public void Create_Uses_Provided_Download_Name()
    {
        var result = ExcelContentResult.Create(new byte[] { 1 }, "custom.xlsx");

        Assert.Equal("custom.xlsx", result.FileDownloadName);
    }
}
