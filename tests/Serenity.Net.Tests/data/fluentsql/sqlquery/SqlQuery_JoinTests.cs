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
    public void Join_Throws_For_Duplicate_Alias_With_Parameterized_Criteria()
    {
        var query = new SqlQuery().From("Base").Select("x")
            .Join(new LeftJoin("Table", "T1", new Criteria("T1") == 5));

        // Duplicate detection compares ToStringIgnoreParams output, where auto
        // values render as deterministic @pN placeholders, so equality with
        // the registered join cannot be verified and re-joining throws instead
        // of risking silently keeping stale values.
        var exception = Assert.Throws<InvalidOperationException>(() =>
            query.Join(new LeftJoin("Table", "T1", new Criteria("T1") == 5)));
        Assert.Contains("already has a join 'T1'", exception.Message);
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
    public void EnsureJoin_And_Join_Aliases_Throw_When_Query_Is_Frozen()
    {
        var alias = new AliasWithJoins("Table", "T1");
        var query = new SqlQuery().From("Base").LeftJoin(alias, null).Select("T0.Id");
        query.Freeze();

        Assert.Throws<InvalidOperationException>(() =>
            query.Join(new LeftJoin("Table", "T1", null)));
        Assert.Throws<InvalidOperationException>(() =>
            query.EnsureJoin(new LeftJoin("Table", "T1", null)));
        Assert.Throws<InvalidOperationException>(() =>
            query.EnsureJoinsInExpression("T1.Id"));
        Assert.Throws<InvalidOperationException>(() =>
            query.LeftJoin(new AliasWithJoins("Table", "T1"), null));
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

    [Fact]
    public void EnsureJoin_Resolves_Join_Dependencies_Without_Local_Join_Map()
    {
        var availableJoins = new Dictionary<string, Join>();
        _ = new LeftJoin(availableJoins, "T2Table", "T2", null);
        var join1 = new LeftJoin("T1Table", "T1", new RawCriteria("T2.Id = T1.Id"));
        var source = new AliasWithJoins("Base", "T0");
        foreach (var pair in availableJoins)
            source.Joins.Add(pair.Key, pair.Value);

        var query = new SqlQuery().From(source).Select("T0.Id");
        query.EnsureJoin(join1);

        var sql = query.ToString();
        Assert.True(sql.IndexOf("T2Table", StringComparison.Ordinal) <
            sql.IndexOf("T1Table", StringComparison.Ordinal));
    }

    [Fact]
    public void SubQuery_Resolves_AutoJoin_From_Parent_Join_Sources()
    {
        var source = new AliasWithJoins("Base", "T0");
        source.Joins.Add("T1", new LeftJoin("Related", "T1", null));
        var query = new SqlQuery().From(source).Select("T0.Id");
        var beforeEnsure = query.ToString();

        var subquery = query.SubQuery().Select("T1.Id");

        Assert.DoesNotContain("LEFT JOIN [Related] T1", subquery.ToString());
        var afterEnsure = query.ToString();
        Assert.DoesNotContain("LEFT JOIN [Related] T1", beforeEnsure);
        Assert.Contains("LEFT JOIN [Related] T1", afterEnsure);
    }

    [Fact]
    public void SubQuery_Uses_Local_Join_Source_Before_Parent_Source()
    {
        var parentSource = new AliasWithJoins("Base", "T0");
        parentSource.Joins.Add("T1", new LeftJoin("ParentRelated", "T1", null));
        var query = new SqlQuery().From(parentSource).Select("T0.Id");

        var subquery = query.SubQuery();
        var subquerySource = new AliasWithJoins("SubBase", "S0");
        subquerySource.Joins.Add("T1", new LeftJoin("SubRelated", "T1", null));
        subquery.From(subquerySource).Select("T1.Id");

        Assert.Contains("LEFT JOIN [SubRelated] T1", subquery.ToString());
        Assert.DoesNotContain("LEFT JOIN [ParentRelated] T1", subquery.ToString());
    }

    [Fact]
    public void Union_Leg_Does_Not_Ensure_Join_From_Previous_Leg()
    {
        var previousLegSource = new AliasWithJoins("Previous", "T0");
        previousLegSource.Joins.Add("T1", new LeftJoin("PreviousRelated", "T1", null));
        var query = new SqlQuery().From(previousLegSource).Select("T0.Id").Union();
        query.Select("T1.Id");

        Assert.DoesNotContain("PreviousRelated", query.ToString());
    }
}
