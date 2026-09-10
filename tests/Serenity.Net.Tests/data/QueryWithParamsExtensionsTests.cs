namespace Serenity.Data;

public class QueryWithParamsExtensionsTests
{
    [Fact]
    public void SetParam_Sets_By_Parameter_Name()
    {
        var query = new SqlQuery();
        var result = query.SetParam(new Parameter("p1"), 5);

        Assert.Same(query, result);
        Assert.Equal(5, query.Params["p1"]);
    }

    [Fact]
    public void AddParam_Adds_And_Returns_Parameter()
    {
        var query = new SqlQuery();
        var param = query.AddParam("value");

        Assert.Equal("value", query.Params[param.Name]);
        Assert.Equal(1, query.ParamCount);
    }
}
