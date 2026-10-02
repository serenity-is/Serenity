namespace Serenity.Data;

public class SqlQuery_SubQueryEmbedTests
{
    private static SqlQuery IndependentSubWithAutoParam()
    {
        // Deliberately never rendered before embedding: auto params only
        // materialize at render time, so the guard must catch these too.
        return new SqlQuery().From("Sub").Select("Id")
            .Where(new Criteria("Sub", "X") == 5);
    }

    [Fact]
    public void In_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new Criteria("T", "Y").In(IndependentSubWithAutoParam()));

        Assert.Contains("SubQuery()", exception.Message);
    }

    [Fact]
    public void NotIn_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new Criteria("T", "Y").NotIn(IndependentSubWithAutoParam()));
    }

    [Fact]
    public void Criteria_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new Criteria(IndependentSubWithAutoParam()));
    }

    [Fact]
    public void Exists_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Criteria.Exists(IndependentSubWithAutoParam()));
    }

    [Fact]
    public void From_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new SqlQuery().From("T").From(IndependentSubWithAutoParam(), new Alias("Sub", "s")));
    }

    [Fact]
    public void Select_Throws_For_Independent_SubQuery_With_Auto_Params()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new SqlQuery().From("T").Select(IndependentSubWithAutoParam(), "SubId"));
    }

    [Fact]
    public void Embed_Allows_Param_Free_Independent_SubQuery()
    {
        var sub = new SqlQuery().From("Sub").Select("Id");

        var query = new SqlQuery().From("T")
            .Where(new Criteria("T", "Y").In(sub))
            .Select(sub, "SubId");

        Assert.Contains("IN (SELECT", query.ToString());
    }

    [Fact]
    public void Embed_Allows_SubQuery_Of_Same_Tree_With_Params()
    {
        var query = new SqlQuery().From("T").Select("T.Id");
        var sub = query.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        query.Where(new Criteria("T", "Y").In(sub));

        Assert.Contains("IN (SELECT", query.ToString());
        Assert.True(query.Params!.ContainsKey("@p1"));
    }

    [Fact]
    public void From_Throws_For_Another_Trees_SubQuery_With_Params()
    {
        var first = new SqlQuery().From("T");
        var sub = first.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        // From knows the outer query, so unlike In() it also rejects
        // subqueries borrowed from another tree.
        Assert.Throws<InvalidOperationException>(() =>
            new SqlQuery().From("T2").From(sub, new Alias("Sub", "s")));
    }

    [Fact]
    public void Select_Throws_For_Another_Trees_SubQuery_With_Params()
    {
        var first = new SqlQuery().From("T");
        var sub = first.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        Assert.Throws<InvalidOperationException>(() =>
            new SqlQuery().From("T2").Select(sub, "SubId"));
    }

    [Fact]
    public void From_Allows_SubQuery_Of_Same_Tree_With_Params()
    {
        var query = new SqlQuery().From("T").Select("T.Id");
        var sub = query.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        query.From(sub, new Alias("Sub", "s"));

        Assert.Contains("SELECT", query.ToString());
        Assert.True(query.Params!.ContainsKey("@p1"));
    }

    [Fact]
    public void Coalesce_Throws_For_Foreign_Query_With_Auto_Params()
    {
        var query = new SqlQuery().From("T");

        Assert.Throws<InvalidOperationException>(() =>
            query.Coalesce(new Criteria("T", "A"), IndependentSubWithAutoParam()));
    }

    [Fact]
    public void Coalesce_Allows_SubQuery_Of_Same_Tree_With_Params()
    {
        var query = new SqlQuery().From("T");
        var sub = query.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        var expression = query.Coalesce(new Criteria("T", "A"), sub);

        Assert.Contains("COALESCE", expression);
        Assert.True(query.Params!.ContainsKey("@p1"));
    }

    [Fact]
    public void Case_Then_Throws_For_Foreign_Query_With_Auto_Params()
    {
        var query = new SqlQuery().From("T");

        Assert.Throws<InvalidOperationException>(() =>
            query.Case(cb => cb
                .When(new Criteria("T", "A") == 1)
                .Then(IndependentSubWithAutoParam())));
    }

    [Fact]
    public void Case_Else_Throws_For_Foreign_Query_With_Auto_Params()
    {
        var query = new SqlQuery().From("T");

        Assert.Throws<InvalidOperationException>(() =>
            query.Case(cb => cb
                .When(new Criteria("T", "A") == 1)
                .Then(2)
                .Else(IndependentSubWithAutoParam())));
    }

    [Fact]
    public void Case_Allows_SubQuery_Of_Same_Tree_With_Params()
    {
        var query = new SqlQuery().From("T");
        var sub = query.SubQuery();
        sub.From("Sub").Select("Id").Where(new Criteria("Sub", "X") == 5);

        var expression = query.Case(cb => cb
            .When(new Criteria("T", "A") == 1)
            .Then(sub));

        Assert.Contains("CASE", expression);
        Assert.True(query.Params!.ContainsKey("@p1"));
    }
}
