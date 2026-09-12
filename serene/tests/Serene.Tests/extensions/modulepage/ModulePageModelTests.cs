namespace Serenity.Web;

public class ModulePageModelTests
{
    [Fact]
    public void Properties_Roundtrip()
    {
        var model = new ModulePageModel
        {
            HtmlMarkup = "<div></div>",
            Options = 5,
            Layout = "_Layout",
            Module = "mod",
            PageId = "id",
            PageTitle = "title"
        };

        Assert.Equal("<div></div>", model.HtmlMarkup);
        Assert.Equal(5, model.Options);
        Assert.Equal("_Layout", model.Layout);
        Assert.Equal("mod", model.Module);
        Assert.Equal("id", model.PageId);
        Assert.Equal("title", model.PageTitle!.ToString());
    }
}
