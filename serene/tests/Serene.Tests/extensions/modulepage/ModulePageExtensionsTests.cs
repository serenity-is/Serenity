namespace Serenity.Web;

public class ModulePageExtensionsTests
{
    private class TestController : Controller
    {
    }

    private static TestController CreateController(out DefaultHttpContext httpContext)
    {
        httpContext = new DefaultHttpContext();
        return new TestController
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext,
                new NullTempDataProvider())
        };
    }

    private class NullTempDataProvider : Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
        {
        }
    }

    [Fact]
    public void PageTitle_Returns_LocalText_Key()
    {
        Assert.Equal("Db." + MockUserRow.Fields.LocalTextPrefix + ".EntityPlural",
            MockUserRow.Fields.PageTitle());
    }

    [Fact]
    public void GridPage_Generic_Creates_Result()
    {
        var controller = CreateController(out _);
        var result = controller.GridPage<MockUserRow>("mod");
        Assert.Equal("<div id=\"GridDiv\"></div>", result.Model.HtmlMarkup);
    }

    [Fact]
    public void GridPage_With_Title_Creates_Result()
    {
        var controller = CreateController(out _);
        var result = controller.GridPage("mod", "Title");
        Assert.Equal("mod", result.Model.Module);
        Assert.Equal("<div id=\"GridDiv\"></div>", result.Model.HtmlMarkup);
    }

    [Fact]
    public void GridPage_With_Model_Uses_Default_Markup()
    {
        var controller = CreateController(out _);
        var model = new ModulePageModel { Module = "mod" };
        var result = controller.GridPage(model);
        Assert.Equal("<div id=\"GridDiv\"></div>", result.Model.HtmlMarkup);
    }

    [Fact]
    public void ModulePage_Throws_For_Null_Model()
    {
        var controller = CreateController(out _);
        Assert.Throws<ArgumentNullException>(() => controller.ModulePage(null!));
    }

    [Fact]
    public void ModulePage_Throws_For_Empty_Module()
    {
        var controller = CreateController(out _);
        Assert.Throws<ArgumentNullException>(() => controller.ModulePage(new ModulePageModel { Module = "" }));
    }

    [Fact]
    public void ModulePage_Transforms_Relative_Module_Path()
    {
        var controller = CreateController(out _);
        var result = controller.ModulePage(new ModulePageModel { Module = "@/Test/Page" });
        Assert.Equal("~/esm/Modules/Test/Page.js", result.Model.Module);
    }

    [Fact]
    public void ModulePage_Does_Not_Append_Js_If_Already()
    {
        var controller = CreateController(out _);
        var result = controller.ModulePage(new ModulePageModel { Module = "@/Test/Page.js" });
        Assert.Equal("~/esm/Modules/Test/Page.js", result.Model.Module);
    }

    [Fact]
    public void PanelPage_Creates_Default_Markup()
    {
        var controller = CreateController(out _);
        var result = controller.PanelPage("mod", "Title");
        Assert.Equal("<div id=\"PanelDiv\"></div>", result.Model.HtmlMarkup);
    }

    [Fact]
    public void PanelPage_With_Model_Creates_Default_Markup()
    {
        var controller = CreateController(out _);
        var result = controller.PanelPage(new ModulePageModel { Module = "mod" });
        Assert.Equal("<div id=\"PanelDiv\"></div>", result.Model.HtmlMarkup);
    }

    [Fact]
    public void PageTitle_Generic_Sets_Title()
    {
        var controller = CreateController(out _);
        var result = controller.GridPage<MockUserRow>("mod").PageTitle<MockUserRow>();
        Assert.Equal("Db." + MockUserRow.Fields.LocalTextPrefix + ".EntityPlural",
            result.Model.PageTitle!.ToString());
    }

    [Fact]
    public void PageTitle_And_Layout_Set_Model_Properties()
    {
        var controller = CreateController(out _);
        var result = controller.GridPage<MockUserRow>("mod")
            .PageTitle("Custom")
            .Layout("_Layout");

        Assert.Equal("Custom", result.Model.PageTitle!.ToString());
        Assert.Equal("_Layout", result.Model.Layout);
    }
}
