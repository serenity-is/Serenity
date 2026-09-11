using System.Collections;

namespace Serenity.Web;

public class LookupScriptTests
{
    private class TestLookupScript : LookupScript
    {
        protected override IEnumerable GetItems() => new object[] { new { id = 1 } };
    }

    [Fact]
    public void GetScriptData_Returns_Items_And_Params()
    {
        var script = new TestLookupScript { LookupKey = "Test" };

        var data = Assert.IsType<LookupScript.Data>(script.GetScriptData());

        Assert.NotNull(data.Items);
        Assert.Empty(data.Params);
    }

    [Fact]
    public void GetScript_Contains_Lookup_Key()
    {
        var script = new TestLookupScript { LookupKey = "Test" };

        Assert.Contains("Lookup.Test", script.GetScript());
        Assert.Equal("Lookup.Test", script.ScriptName);
    }

    [Fact]
    public void Field_Properties_RoundTrip()
    {
        var script = new TestLookupScript();

        Assert.Null(script.IdField);
        Assert.Null(script.TextField);
        Assert.Null(script.ParentIdField);

        script.IdField = "Id";
        script.TextField = "Name";
        script.ParentIdField = "Parent";

        Assert.Equal("Id", script.IdField);
        Assert.Equal("Name", script.TextField);
        Assert.Equal("Parent", script.ParentIdField);
        Assert.Equal("Id", script.LookupParams["idField"]);
        Assert.Equal("Name", script.LookupParams["textField"]);
        Assert.Equal("Parent", script.LookupParams["parentIdField"]);
    }

    [Fact]
    public void Field_Properties_Return_Null_For_Null_Values()
    {
        var script = new TestLookupScript();
        script.LookupParams["idField"] = null;

        Assert.Null(script.IdField);
    }
}
