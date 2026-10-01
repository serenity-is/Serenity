namespace Serenity.Data;

public class QueryWithParamsExtensionsTests
{
    [Fact]
    public void SetParam_Sets_By_Parameter_Name()
    {
        var query = new SqlQuery();
        var result = query.SetParam(new Parameter("@p1"), 5);

        Assert.Same(query, result);
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void SetParam_And_AddParam_Reject_Names_Without_At_Prefix()
    {
        var query = new SqlQuery();

        Assert.Throws<ArgumentOutOfRangeException>(() => query.SetParam("p1", 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => query.AddParam("p1", 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Parameter("p1"));
        Assert.Throws<ArgumentNullException>(() => query.SetParam("", 5));
    }

    [Theory]
    [InlineData("@")]
    [InlineData("@ ")]
    [InlineData("@\t")]
    public void Parameter_Rejects_AtOnly_And_Blank_Names(string name)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Parameter(name));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ParamCriteria(name));
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
        query.SetParam("@p1", 5);
        Assert.False(((IQueryWithParams)query).IsParamsFrozen);
        var parameters = query.Params!;
        Assert.Throws<NotSupportedException>(() =>
            ((IDictionary<string, object?>)parameters).Add("p2", 6));

        var frozen = query.FreezeParams();

        Assert.Same(query, frozen);
        Assert.Same(query, frozen.FreezeParams());
        Assert.True(((IQueryWithParams)query).IsParamsFrozen);
        Assert.Equal(5, query.Params["@p1"]);
        Assert.Throws<InvalidOperationException>(() => query.AddParam("@p2", 6));
        Assert.Throws<InvalidOperationException>(() => query.SetParam("@p1", 6));
        Assert.Same(query, query.Select("c"));
    }

    [Fact]
    public void AutoParam_WhenParamsFrozen_Throws_Without_Advancing_Counter()
    {
        var query = new SqlQuery();
        query.FreezeParams();

        Assert.Throws<InvalidOperationException>(() => query.AutoParam());
        Assert.False(query.HasAutoParams);
    }

    [Fact]
    public void AutoParam_WhenQueryFrozen_Throws_Without_Advancing_Counter()
    {
        var query = new SqlQuery().Select("c");
        query.Freeze();

        Assert.Throws<InvalidOperationException>(() => query.AutoParam());
        Assert.False(query.HasAutoParams);
    }

    [Fact]
    public void Freeze_Freezes_Query_And_Subquery_Tree()
    {
        var query = new SqlQuery().Select("c").From("t");
        var subQuery = query.SubQuery().Select("d").From("u");

        var exception = Assert.Throws<InvalidOperationException>(() => subQuery.Freeze());

        Assert.Equal("Freeze cannot be called on a subquery.", exception.Message);
        Assert.False(((IQueryWithParams)query).IsFrozen);
        Assert.False(((IQueryWithParams)subQuery).IsFrozen);
        Assert.False(query.IsParamsFrozen);
        Assert.False(subQuery.IsParamsFrozen);

        ((IQueryWithParams)query).Freeze();

        Assert.True(((IQueryWithParams)query).IsFrozen);
        Assert.True(((IQueryWithParams)subQuery).IsFrozen);
        Assert.True(query.IsParamsFrozen);
        Assert.True(subQuery.IsParamsFrozen);
        Assert.Throws<InvalidOperationException>(() => query.Select("e"));
        Assert.Throws<InvalidOperationException>(() => subQuery.Select("f"));
        Assert.Throws<InvalidOperationException>(() => subQuery.Where(new Criteria("A") == 1));
        Assert.Equal(0, query.ParamCount);
    }

    [Fact]
    public void Freeze_Extension_Works_For_Insert_Update_And_Delete()
    {
        var insert = new SqlInsert("T").SetTo("A", "1");
        var update = new SqlUpdate("T").SetTo("A", "1");
        var delete = new SqlDelete("T");

        Assert.Same(insert, insert.Freeze());
        Assert.Same(update, update.Freeze());
        Assert.Same(delete, delete.Freeze());

        Assert.Throws<InvalidOperationException>(() => insert.SetTo("B", "2"));
        Assert.Throws<InvalidOperationException>(() => update.SetTo("B", "2"));
        Assert.Throws<InvalidOperationException>(() => delete.Where(new Criteria("A") == 1));
    }

    [Fact]
    public void FreezeParams_Throws_On_SubQuery()
    {
        var query = new SqlQuery();
        query.SetParam("@p1", 5);
        var subQuery = query.SubQuery();

        Assert.False(((IQueryWithParams)subQuery).IsParamsFrozen);
        var exception = Assert.Throws<InvalidOperationException>(() => subQuery.FreezeParams());

        Assert.Equal("FreezeParams cannot be called on a subquery.", exception.Message);
        Assert.False(((IQueryWithParams)query).IsParamsFrozen);
        Assert.False(((IQueryWithParams)subQuery).IsParamsFrozen);
        query.SetParam("@p1", 6);
        Assert.Equal(6, query.Params["@p1"]);
    }

    [Fact]
    public void FreezeParams_Can_Be_Called_Through_IQueryWithParams()
    {
        var query = new SqlQuery();
        query.SetParam("@p1", 5);
        IQueryWithParams queryWithParams = query;

        queryWithParams.FreezeParams();

        Assert.True(queryWithParams.IsParamsFrozen);
        Assert.Throws<InvalidOperationException>(() => query.SetParam("@p1", 6));
    }
}
