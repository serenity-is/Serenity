namespace Serenity.Data;

public class SqlQuery_Where_Tests
{
    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    [Fact]
    public void WhereDoesAndWhenCalledMoreThanOnce()
    {
        var query = new SqlQuery().From("t").Select("c").Where("x > 5").Where("y < 4");
        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM [t] WHERE x > 5 AND y < 4"),
            Normalize.Sql(
                query.ToString())
        );
    }

    [Fact]
    public void WhereStoresCriteriaAndStringExtensionWrapsStrings()
    {
        var query = new SqlQuery().From("t");
        var criteria = new Criteria("x > 5");
        query.Where(criteria);

        Assert.Same(criteria, Assert.Single(((IFilterableQuery)query).GetWhereCriteria()));
        Assert.Equal("x > 5", ((ISqlQuery)query).Where);

        query.Where("y < 4");
        Assert.IsType<Criteria>(((IFilterableQuery)query).GetWhereCriteria()[1]);
        Assert.Equal("x > 5 AND y < 4", ((ISqlQuery)query).Where);
    }

    [Fact]
    public void WhereWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where((string)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where(String.Empty));
    }

    [Fact]
    public void GetWhereClauseKeepsQueryAliases()
    {
        var query = new SqlQuery().Where("T0.ID = 5");

        Assert.Equal("T0.ID = 5", ((IFilterableQuery)query).GetWhereClause());
        Assert.Equal("T0.ID = 5", ((ISqlQuery)query).Where);
    }

    [Fact]
    public void Where_Ensures_Referenced_Joins()
    {
        var root = new AliasWithJoins("Base", "T0");
        _ = new LeftJoin(root.Joins, "Related", "T1", null);

        var query = new SqlQuery().From(root).Select("T0.Id")
            .Where(new Criteria("T1.Value") > 0);

        Assert.Contains("LEFT JOIN [Related] T1", Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void Where_Renders_Parameters_Once()
    {
        var query = new SqlQuery().Select("A").From("T").Where(new Criteria("A") == 5);
        Assert.Equal(1, query.ParamCount);

        var where = ((IFilterableQuery)query).GetWhereClause();

        Assert.Equal(1, query.ParamCount);
        Assert.Equal(where, ((IFilterableQuery)query).GetWhereClause());
        Assert.Contains(where, query.ToString());
        Assert.Equal(1, query.ParamCount);
    }
}
