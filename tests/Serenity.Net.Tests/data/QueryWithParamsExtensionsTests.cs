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

    [Fact]
    public void FreezeParams_Returns_Query_And_Prevents_Parameter_Mutations()
    {
        var query = new SqlQuery();
        query.SetParam("p1", 5);
        Assert.False(((IQueryWithParams)query).IsParamsFrozen);

        var frozen = query.FreezeParams();

        Assert.Same(query, frozen);
        Assert.Same(query, frozen.FreezeParams());
        Assert.True(((IQueryWithParams)query).IsParamsFrozen);
        Assert.Equal(5, query.Params["p1"]);
        Assert.Throws<InvalidOperationException>(() => query.AddParam("p2", 6));
        Assert.Throws<InvalidOperationException>(() => query.SetParam("p1", 6));
    }

    [Fact]
    public void FreezeParams_On_SubQuery_Freezes_Shared_Root_Params()
    {
        var query = new SqlQuery();
        query.SetParam("p1", 5);
        var subQuery = query.SubQuery();

        Assert.False(((IQueryWithParams)subQuery).IsParamsFrozen);
        Assert.Same(subQuery, subQuery.FreezeParams());
        Assert.True(((IQueryWithParams)query).IsParamsFrozen);
        Assert.True(((IQueryWithParams)subQuery).IsParamsFrozen);
        Assert.Throws<InvalidOperationException>(() => query.SetParam("p1", 6));
        Assert.Throws<InvalidOperationException>(() => subQuery.AddParam("p2", 6));
    }

    [Fact]
    public void FreezeParams_Can_Be_Called_Through_IQueryWithParams()
    {
        var query = new SqlQuery();
        query.SetParam("p1", 5);
        IQueryWithParams queryWithParams = query;

        queryWithParams.FreezeParams();

        Assert.True(queryWithParams.IsParamsFrozen);
        Assert.Throws<InvalidOperationException>(() => query.SetParam("p1", 6));
    }
}
