namespace Serenity.Data;

public class SqlQuery_JoinTests
{
    private sealed class RawCriteria(string text) : ICriteria
    {
        public bool IsEmpty => string.IsNullOrEmpty(text);
        public string ToString(IQueryWithParams query) => text;
        public void ToString(StringBuilder sb, IQueryWithParams query) => sb.Append(text);
        public string ToStringIgnoreParams() => text;
    }

    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    private sealed class JoinWithJoins(string table, string name, ICriteria? criteria)
        : LeftJoin(new Dictionary<string, Join>(), table, name, criteria!), IHaveJoins
    {
        IDictionary<string, Join> IHaveJoins.Joins => Joins!;
    }

    [Fact]
    public void LeftJoin_With_Table_Validates_And_Joins()
    {
        var alias = new AliasWithJoins("Table", "T1");

        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().LeftJoin("Table", null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().LeftJoin("", alias, null!));

        var query = new SqlQuery().From("Base").Select("x")
            .LeftJoin("Table", alias, null);

        Assert.Contains("LEFT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void LeftJoin_With_Alias_Validates_And_Joins()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().LeftJoin(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().LeftJoin(new Alias("", "T1"), null!));

        var query = new SqlQuery().From("Base").Select("x")
            .LeftJoin(new Alias("Table", "T1"), null);

        Assert.Contains("LEFT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void RightJoin_With_Table_Validates_And_Joins()
    {
        var alias = new AliasWithJoins("Table", "T1");

        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().RightJoin("Table", null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().RightJoin("", alias, null!));

        var query = new SqlQuery().From("Base").Select("x")
            .RightJoin("Table", alias, null);

        Assert.Contains("RIGHT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void RightJoin_With_Alias_Validates_And_Joins()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().RightJoin(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().RightJoin(new Alias("", "T1"), null!));

        var query = new SqlQuery().From("Base").Select("x")
            .RightJoin(new AliasWithJoins("Table", "T1"), null);

        Assert.Contains("RIGHT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void InnerJoin_Validates_And_Joins()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().InnerJoin(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SqlQuery().InnerJoin(new Alias("", "T1"), null!));

        var query = new SqlQuery().From("Base").Select("x")
            .InnerJoin(new AliasWithJoins("Table", "T1"), null);

        Assert.Contains("INNER JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void Join_With_NonBinary_Criteria_Wraps_In_Parens()
    {
        var query = new SqlQuery().From("Base").Select("x")
            .Join(new LeftJoin("Table", "T1", new RawCriteria("a = 1")));

        Assert.Contains("ON (a = 1)", query.ToString());
    }

    [Fact]
    public void Join_Registers_Alias_And_AliasWithJoins()
    {
        var alias = new AliasWithJoins("Table", "T1");

        var query = new SqlQuery().From("Base").Select("x")
            .LeftJoin(alias, null);

        Assert.Contains("LEFT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void Join_With_IHaveJoins_Join_Registers_AliasWithJoins()
    {
        var join = new JoinWithJoins("Table", "T1", null);

        var query = new SqlQuery().From("Base").Select("x").Join(join);

        Assert.Contains("LEFT JOIN [Table] T1", query.ToString());
    }

    [Fact]
    public void Join_Throws_For_Conflicting_Alias_And_Is_Idempotent()
    {
        var query = new SqlQuery().From("Base").Select("x")
            .Join(new LeftJoin("Table", "T1", null));

        Assert.Same(query, query.Join(new LeftJoin("Table", "T1", null)));

        Assert.Throws<InvalidOperationException>(() =>
            query.Join(new LeftJoin("Other", "T1", null)));
    }

    [Fact]
    public void Join_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Join(null!));
    }

    [Fact]
    public void EnsureJoin_And_EnsureJoinsInExpression_Work()
    {
        var query = new SqlQuery().From("Base").Select("x");

        Assert.Same(query, query.EnsureJoinsInExpression(""));
        Assert.Same(query, query.EnsureJoin(new LeftJoin("Table", "T1", null)));
        Assert.Same(query, query.EnsureJoin(new LeftJoin("Table", "T1", null)));
    }

    [Fact]
    public void EnsureJoin_Recurses_Into_Referenced_Joins()
    {
        var joins = new Dictionary<string, Join>();
        _ = new LeftJoin(joins, "T2Table", "T2", null);
        var join1 = new LeftJoin(joins, "T1Table", "T1", new RawCriteria("T2.a = 1"));

        var query = new SqlQuery().From("Base").Select("x");

        Assert.Same(query, query.EnsureJoin(join1));
        Assert.Contains("T2Table", query.ToString());
    }
}
