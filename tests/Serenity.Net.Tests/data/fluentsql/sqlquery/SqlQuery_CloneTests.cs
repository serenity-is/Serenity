namespace Serenity.Data;

public class SqlQuery_CloneTests
{
    [Fact]
    public void Clone_Copies_Full_Query()
    {
        var query = new SqlQuery()
            .Dialect(new SqlServer2012Dialect())
            .From("TestTable")
            .Select("A", "B")
            .Where("C = 1")
            .OrderBy("A")
            .GroupBy("B")
            .Having("Count(*) > 1")
            .Skip(5)
            .Take(10)
            .Distinct(true);

        var clone = query.Clone();

        Assert.Equal(query.ToString(), clone.ToString());
        Assert.Equal(query.ParamCount, clone.ParamCount);
    }

    [Fact]
    public void Clone_Copies_Params_For_Root_Query()
    {
        var query = new SqlQuery().From("T").Select("T.Id").Where("A = @p1");
        query.AddParam("@p1", 1);

        var clone = query.Clone();

        Assert.Equal(1, clone.ParamCount);
        Assert.Equal(query.ToString(), clone.ToString());
    }

    [Fact]
    public void CloneParams_Preserves_Dialect_And_AutoParameter_Counter()
    {
        var query = new SqlQuery().Dialect(PostgresDialect.Instance);
        var first = query.AddParam(1);

        var clone = query.Clone();
        var second = clone.AddParam(2);

        Assert.Equal("@p1", first.Name);
        Assert.Equal("@p2", second.Name);
        Assert.Same(PostgresDialect.Instance, clone.Dialect());
        Assert.True(clone.IsDialectOverridden);
        Assert.Equal(2, clone.ParamCount);
        Assert.Equal(1, query.ParamCount);
    }

    [Fact]
    public void Clone_Copies_SubQuery_With_Parent()
    {
        var query = new SqlQuery().From("Parent");
        var sub = query.SubQuery()
            .Select("A")
            .From("SubTable")
            .Where(new Criteria("B") == 1);

        _ = sub.ToString();

        var clone = sub.Clone();

        Assert.Equal(sub.ToString(), clone.ToString());

        var next = clone.AddParam(2);
        Assert.Equal("@p2", next.Name);
        Assert.Equal(2, query.ParamCount);
        Assert.Same(query, clone.Parent);
    }

    [Fact]
    public void Clone_Of_SubQuery_Uses_Current_Parent_Dialect()
    {
        var query = new SqlQuery();
        var sub = query.SubQuery().Select("A").From("T");
        query.Dialect(PostgresDialect.Instance);

        var clone = sub.Clone();

        Assert.Same(query, clone.Parent);
        Assert.Same(PostgresDialect.Instance, clone.Dialect());
        Assert.True(clone.IsDialectOverridden);
    }

    [Fact]
    public void Clone_DeepClones_And_Reparents_UnionQuery()
    {
        var query = new SqlQuery().Select("a").From("t").Union();
        query.Select("b").From("u");
        var originalSql = query.ToString();

        var clone = query.Clone();
        var cloneUnion = (SqlQuery)((ISqlQuery)clone).UnionQuery!;
        var originalUnion = ((ISqlQuery)query).UnionQuery;

        Assert.NotSame(originalUnion, cloneUnion);
        Assert.Same(clone, ((ISqlQuery)cloneUnion).Parent);

        cloneUnion.Select("c");

        Assert.Equal(originalSql, query.ToString());
        Assert.NotEqual(originalSql, clone.ToString());
        Assert.Contains("a,\nc", clone.ToString());
    }

    [Fact]
    public void Clone_FrozenClone_Prevents_UnionQueryMutation_WithoutChangingSource()
    {
        var query = new SqlQuery().Select("a").From("t").Union();
        query.Select("b").From("u");
        var originalSql = query.ToString();

        var clone = query.Clone();
        ((IQueryWithParams)clone).Freeze();
        var cloneUnion = (SqlQuery)((ISqlQuery)clone).UnionQuery!;

        Assert.Throws<InvalidOperationException>(() => cloneUnion.Select("c"));
        Assert.Equal(originalSql, query.ToString());
        Assert.Equal(originalSql, clone.ToString());
    }

    [Fact]
    public void Clone_Copies_Alias_Expressions_And_AliasWithJoins()
    {
        var query = new SqlQuery().From("Base").Select("x")
            .Join(new LeftJoin("T1Table", "T1", null))
            .LeftJoin(IdNameRow.Fields, null);

        var clone = query.Clone();

        Assert.Equal(query.ToString(), clone.ToString());
    }
}
