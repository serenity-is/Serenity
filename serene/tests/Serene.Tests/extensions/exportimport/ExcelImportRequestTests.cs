namespace Serenity.Extensions;

public class ExcelImportRequestTests
{
    [Fact]
    public void Request_Properties_Roundtrip()
    {
        var request = new ExcelImportRequest { FileName = "file.xlsx" };
        Assert.Equal("file.xlsx", request.FileName);
    }

    [Fact]
    public void Response_Properties_Roundtrip()
    {
        var response = new ExcelImportResponse
        {
            Inserted = 1,
            Updated = 2,
            ErrorList = ["err"]
        };

        Assert.Equal(1, response.Inserted);
        Assert.Equal(2, response.Updated);
        Assert.Equal(["err"], response.ErrorList);
    }
}
