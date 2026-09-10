namespace Serenity.Data;

public class JoinTests
{
    [Fact]
    public void LeftJoin_GetKeyword_Returns_LeftJoin()
    {
        Assert.Equal("LEFT JOIN", new LeftJoin("Table", "T1", null).GetKeyword());
    }

    [Fact]
    public void InnerJoin_GetKeyword_Returns_InnerJoin()
    {
        Assert.Equal("INNER JOIN", new InnerJoin("Table", "T1", null).GetKeyword());
    }

    [Fact]
    public void RightJoin_GetKeyword_Returns_RightJoin()
    {
        Assert.Equal("RIGHT JOIN", new RightJoin("Table", "T1", null).GetKeyword());
    }

    [Fact]
    public void CrossApply_GetKeyword_Returns_CrossApply()
    {
        Assert.Equal("CROSS APPLY", new CrossApply("SELECT 1", "T1").GetKeyword());
    }

    [Fact]
    public void OuterApply_GetKeyword_Returns_OuterApply()
    {
        Assert.Equal("OUTER APPLY", new OuterApply("SELECT 1", "T1").GetKeyword());
    }

    [Fact]
    public void Join_Registers_Itself_In_Joins_Dictionary()
    {
        var joins = new Dictionary<string, Join>();
        var join = new LeftJoin(joins, "Table", "T1", null);

        Assert.Contains("T1", joins.Keys);
        Assert.Same(join, joins["T1"]);
        Assert.Same(joins, join.Joins);
    }

    [Fact]
    public void Join_Without_Joins_Dictionary_Has_Null_Joins()
    {
        var join = new LeftJoin("Table", "T1", null);

        Assert.Null(join.Joins);
    }

    [Fact]
    public void Join_With_Duplicate_Alias_Throws_ArgumentException()
    {
        var joins = new Dictionary<string, Join>();
        new LeftJoin(joins, "Table", "T1", null);

        var ex = Assert.Throws<ArgumentException>(() =>
            new LeftJoin(joins, "Other", "T1", null));

        Assert.Contains("There is already a join with alias 'T1'", ex.Message);
    }

    [Fact]
    public void Join_Name_Table_And_OnCriteria_Properties()
    {
        var criteria = new Criteria("T1", "a") == new Criteria(0, "b");
        var join = new LeftJoin("Table", "T1", criteria);

        Assert.Equal("T1", join.Name);
        Assert.Equal("T1.", join.NameDot);
        Assert.Equal("Table", join.Table);
        Assert.Same(criteria, join.OnCriteria);
    }

    [Fact]
    public void Join_OnCriteria_Is_Null_When_Not_Passed()
    {
        var join = new LeftJoin("Table", "T1", null);

        Assert.Null(join.OnCriteria);
    }

    [Fact]
    public void Join_ReferencedAliases_Are_Located_From_OnCriteria()
    {
        var join = new LeftJoin("Table", "T3",
            new Criteria("T1", "a") == new Criteria("T2", "b"));

        Assert.NotNull(join.ReferencedAliases);
        Assert.Contains("T1", join.ReferencedAliases);
        Assert.Contains("T2", join.ReferencedAliases);
        Assert.Equal(2, join.ReferencedAliases.Count);
    }

    [Fact]
    public void Join_ReferencedAliases_Are_Located_From_Table()
    {
        var join = new LeftJoin("T1.SubTable", "T2", null);

        Assert.NotNull(join.ReferencedAliases);
        Assert.Contains("T1", join.ReferencedAliases);
        Assert.Single(join.ReferencedAliases);
    }

    [Fact]
    public void Join_ReferencedAliases_Merge_Criteria_And_Table_Aliases()
    {
        var join = new LeftJoin("T1.SubTable", "T3",
            new Criteria("T2", "x") == 1);

        Assert.NotNull(join.ReferencedAliases);
        Assert.Contains("T1", join.ReferencedAliases);
        Assert.Contains("T2", join.ReferencedAliases);
    }

    [Fact]
    public void Join_ReferencedAliases_Are_Null_Without_Alias_References()
    {
        var join = new LeftJoin("Table", "T1", null);

        Assert.Null(join.ReferencedAliases);
    }

    [Fact]
    public void Join_Accepts_Criteria_With_Params_Via_ToStringIgnoreParams()
    {
        var criteria = new Criteria("T1", "x") == new Parameter("@p1");
        var join = new LeftJoin("Table", "T1", criteria);

        Assert.Contains("T1", join.ReferencedAliases);
        Assert.Equal("(T1.[x] = @p1)", join.OnCriteria!.ToStringIgnoreParams());
    }

    [Fact]
    public void Join_RowType_Can_Be_Set()
    {
        var join = new LeftJoin("Table", "T1", null);

        Assert.Null(join.RowType);

        join.RowType = typeof(string);

        Assert.Equal(typeof(string), join.RowType);
    }

    [Fact]
    public void Join_Inherits_Alias_Members()
    {
        var join = new LeftJoin("Table", "T1", null);

        Assert.Equal("T1.Field", join["Field"]);
        Assert.Equal(Alias.T1.Name, join.Name);
    }

    [Fact]
    public void CrossApply_Wraps_SubQuery_In_Parentheses()
    {
        var join = new CrossApply("SELECT 1 AS y", "ca");

        Assert.Equal("(SELECT 1 AS y)", join.Table);
        Assert.Equal("ca", join.Name);
        Assert.Null(join.OnCriteria);
    }

    [Fact]
    public void CrossApply_Empty_SubQuery_Is_Kept_As_Is()
    {
        var join = new CrossApply("", "ca");

        Assert.Equal("", join.Table);
    }

    [Fact]
    public void OuterApply_Wraps_SubQuery_In_Parentheses()
    {
        var join = new OuterApply("SELECT 1 AS y", "oa");

        Assert.Equal("(SELECT 1 AS y)", join.Table);
        Assert.Equal("oa", join.Name);
        Assert.Null(join.OnCriteria);
    }

    [Fact]
    public void OuterApply_Empty_SubQuery_Is_Kept_As_Is()
    {
        var join = new OuterApply("", "oa");

        Assert.Equal("", join.Table);
    }

    [Fact]
    public void SqlQuery_Renders_LeftJoin_With_OnCriteria()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .LeftJoin(new Alias("Table", "T1"),
                new Criteria("T1", "a") == new Criteria(0, "b"));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] LEFT JOIN [Table] T1 ON (T1.[a] = T0.[b])"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_Join_From_Joins_Dictionary()
    {
        var joins = new Dictionary<string, Join>();
        var join = new LeftJoin(joins, "Table", "T1",
            new Criteria("T1", "a") == new Criteria(0, "b"));

        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(join);

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] LEFT JOIN [Table] T1 ON (T1.[a] = T0.[b])"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_InnerJoin_Keyword()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new InnerJoin("Table", "T1",
                new Criteria("T1", "a") == new Criteria(0, "b")));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] INNER JOIN [Table] T1 ON (T1.[a] = T0.[b])"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_RightJoin_Keyword()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new RightJoin("Table", "T1",
                new Criteria("T1", "a") == new Criteria(0, "b")));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] RIGHT JOIN [Table] T1 ON (T1.[a] = T0.[b])"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_CrossApply_Keyword()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new CrossApply("SELECT 1 AS y", "ca"));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] CROSS APPLY (SELECT 1 AS y) ca"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_OuterApply_Keyword()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new OuterApply("SELECT 1 AS y", "oa"));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] OUTER APPLY (SELECT 1 AS y) oa"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_Join_Without_OnCriteria()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new LeftJoin("Table", "T1", null));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] LEFT JOIN [Table] T1"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void SqlQuery_Renders_Join_Criteria_Referencing_Other_Joins()
    {
        var query = new SqlQuery()
            .From("Base")
            .Select("x")
            .Join(new LeftJoin("T1Table", "T1", null))
            .Join(new LeftJoin("T2Table", "T2",
                new Criteria("T2", "a") == new Criteria("T1", "b")));

        Assert.Equal(
            Normalize.Sql("SELECT x FROM [Base] LEFT JOIN [T1Table] T1 " +
                "LEFT JOIN [T2Table] T2 ON (T2.[a] = T1.[b])"),
            Normalize.Sql(query.ToString()));
    }
}
