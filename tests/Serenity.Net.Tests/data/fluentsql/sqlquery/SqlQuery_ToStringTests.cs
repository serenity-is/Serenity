namespace Serenity.Data;

public class SqlQuery_ToStringTests
{
    private sealed class QueryToStringDialect : SqlServer2012Dialect, ISqlQueryToString
    {
        public int CallCount { get; private set; }

        public string ToString(ISqlQuery sqlQuery)
        {
            CallCount++;
            return "CUSTOM";
        }
    }

    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    private sealed class CachedParentQuery : QueryWithParams
    {
        public CachedParentQuery()
        {
            Child = CreateSubQuery<SqlQuery>();
        }

        public SqlQuery Child { get; }

        public override string ToString() => cachedToString ??= Child.ToString();
    }

    [Fact]
    public void ToString_Uses_ISqlQueryToString_Dialect_If_Available()
    {
        var query = new SqlQuery()
            .Dialect(new QueryToStringDialect())
            .Select("c")
            .From("t");

        Assert.Equal("CUSTOM", query.ToString());
    }

    [Fact]
    public void ToString_Caches_Except_For_Query_Shape_Changes()
    {
        var dialect = new QueryToStringDialect();
        var query = new SqlQuery()
            .Dialect(dialect)
            .Select("c")
            .From("t");

        Assert.Equal("CUSTOM", query.ToString());
        query.SetParam("@p", 1);
        query.SetParam("@p", 2);
        Assert.Equal("CUSTOM", query.ToString());
        Assert.Equal(1, dialect.CallCount);

        query.Distinct(true);
        Assert.Equal("CUSTOM", query.ToString());
        Assert.Equal(2, dialect.CallCount);
    }

    [Fact]
    public void ToString_Invalidates_For_Query_Shape_Mutations()
    {
        (string Name, Action<SqlQuery> Mutate)[] mutations =
        [
            ("Distinct", query => query.Distinct(true)),
            ("ForXml", query => query.ForXml("RAW")),
            ("ForJson", query => query.ForJson("PATH")),
            ("From string", query => query.From("Other")),
            ("From alias", query => query.From(new Alias("Other", "T1"))),
            ("From table and alias", query => query.From("Other", new Alias("Other", "T1"))),
            ("From subquery", query => query.From(new SqlQuery().Select("d").From("Other"), new Alias("Other", "T1"))),
            ("GroupBy string", query => query.GroupBy("c")),
            ("GroupBy alias", query => query.GroupBy(new Alias("T0"), "c")),
            ("Having", query => query.Having("c > 0")),
            ("OrderBy string", query => query.OrderBy("c")),
            ("OrderBy alias", query => query.OrderBy(new Alias("T0"), "c")),
            ("OrderByFirst", query => query.OrderByFirst("c")),
            ("Select string", query => query.Select("d")),
            ("Select alias", query => query.Select(new Alias("T0"), "d")),
            ("Select with column name", query => query.Select("d", "D")),
            ("Select alias with column name", query => query.Select(new Alias("T0"), "d", "D")),
            ("Select subquery", query => query.Select(new SqlQuery().Select("d").From("Other"))),
            ("Select subquery with column name", query => query.Select(new SqlQuery().Select("d").From("Other"), "D")),
            ("SelectMany", query => query.SelectMany("d", "e")),
            ("Skip", query => query.Skip(1)),
            ("Take", query => query.Take(1)),
            ("Union", query => query.Union()),
            ("Where", query => query.Where(new Criteria("c") == 1)),
            ("OmitParens", query => query.OmitParens()),
            ("CountRecords", query => query.CountRecords = true),
            ("Join", query => query.Join(new LeftJoin("Other", "T1", null))),
            ("LeftJoin table", query => query.LeftJoin("Other", new Alias("Other", "T1"), null)),
            ("LeftJoin alias", query => query.LeftJoin(new Alias("Other", "T1"), null)),
            ("RightJoin table", query => query.RightJoin("Other", new Alias("Other", "T1"), null)),
            ("RightJoin alias", query => query.RightJoin(new Alias("Other", "T1"), null)),
            ("InnerJoin alias", query => query.InnerJoin(new Alias("Other", "T1"), null)),
            ("EnsureJoin", query => query.EnsureJoin(new LeftJoin("Other", "T1", null))),
            ("FullTextSearchJoin", query => query.FullTextSearchJoin("Other", "c", "word", "T0", "Id", "CT")),
            ("Column constructor", query => _ = new SqlQuery.Column(query, "d", null, null))
        ];

        foreach (var (name, mutate) in mutations)
        {
            var dialect = new QueryToStringDialect();
            var query = new SqlQuery()
                .Dialect(dialect)
                .Select("c")
                .From("t");

            Assert.Equal("CUSTOM", query.ToString());
            mutate(query);
            Assert.Equal("CUSTOM", query.ToString());
            Assert.True(dialect.CallCount == 2, $"{name} did not invalidate the cached SQL.");
        }
    }

    [Fact]
    public void ToString_Invalidates_When_Dialect_Changes()
    {
        var initialDialect = new QueryToStringDialect();
        var query = new SqlQuery()
            .Dialect(initialDialect)
            .Select("c")
            .From("t");

        Assert.Equal("CUSTOM", query.ToString());

        var replacementDialect = new QueryToStringDialect();
        query.Dialect(replacementDialect);

        Assert.Equal("CUSTOM", query.ToString());
        Assert.Equal(1, initialDialect.CallCount);
        Assert.Equal(1, replacementDialect.CallCount);
    }

    [Fact]
    public void ToString_Invalidates_When_Ensuring_Referenced_Join()
    {
        var dialect = new QueryToStringDialect();
        var alias = new AliasWithJoins("Base", "T1");
        alias.Joins.Add("T2", new LeftJoin("Related", "T2", null));
        var query = new SqlQuery()
            .Dialect(dialect)
            .Select("c")
            .From("t")
            .InnerJoin(alias, null);

        Assert.Equal("CUSTOM", query.ToString());

        query.EnsureJoinsInExpression("T2.c");

        Assert.Equal("CUSTOM", query.ToString());
        Assert.Equal(2, dialect.CallCount);
    }

    [Fact]
    public void ToString_Invalidates_Parent_QueryWithParams_When_Child_Changes()
    {
        var parent = new CachedParentQuery();
        parent.Child.Select("c").From("t");
        var initial = parent.ToString();

        parent.Child.Select("d");
        var updated = parent.ToString();

        Assert.NotSame(initial, updated);
        Assert.Contains("d", updated);
    }

    [Fact]
    public void Query_Column_And_Order_Views_Are_ReadOnly()
    {
        var query = new SqlQuery().Select("c").From("t");
        var extensible = (ISqlQueryExtensible)query;
        var sqlQuery = (ISqlQuery)query;

        Assert.Single(extensible.Columns);
        Assert.Throws<NotSupportedException>(() => ((IList<SqlQuery.Column>)extensible.Columns)
            .Add(new SqlQuery.Column("d", null, -1, null)));
        Assert.Throws<NotSupportedException>(() => ((IList<SqlQuery.Column>)sqlQuery.Columns)
            .Add(new SqlQuery.Column("d", null, -1, null)));

        query.OrderBy("c");
        Assert.Single(sqlQuery.OrderBy);
        Assert.Throws<NotSupportedException>(() => ((IList<string>)sqlQuery.OrderBy).Add("d"));
    }

    [Fact]
    public void ToString_Invalidates_When_FromOrUnionChildChanges()
    {
        var query = new SqlQuery().Select("c").From("t");
        var original = query.ToString();

        query.FullTextSearchJoin("t", "c", "word", "t", "Id", "CT");
        var withFullText = query.ToString();
        Assert.NotEqual(original, withFullText);
        Assert.Contains("CONTAINSTABLE", withFullText);

        var union = new SqlQuery().Select("a").From("t").Union();
        union.Select("b").From("u");
        var beforeUnionChange = union.ToString();
        ((SqlQuery)((ISqlQuery)union).UnionQuery!).Select("c");
        var afterUnionChange = union.ToString();

        Assert.NotEqual(beforeUnionChange, afterUnionChange);
        Assert.Contains("a,\nc", afterUnionChange);
    }

    [Fact]
    public void ToString_Throws_For_Null_Query()
    {
        Assert.Throws<ArgumentNullException>(() => SqlQuery.ToString(null!, SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void ToString_Appends_ForXml()
    {
        var sql = new SqlQuery().Select("c").From("t").ForXml("RAW").ToString();
        Assert.Contains("FOR XML RAW", sql);
    }

    [Fact]
    public void ToString_Appends_ForJson()
    {
        var sql = new SqlQuery().Select("c").From("t").ForJson().ToString();
        Assert.Contains("FOR JSON AUTO", sql);

        var sql2 = new SqlQuery().Select("c").From("t").ForJson("PATH").ToString();
        Assert.Contains("FOR JSON PATH", sql2);
    }

    [Fact]
    public void ToString_Distinct_With_Multiple_Columns()
    {
        var sql = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("a")
            .Select("b")
            .From("t")
            .Distinct(true)
            .ToString();

        Assert.Contains("SELECT DISTINCT", sql);
    }

    [Fact]
    public void ToString_SkipKeyword_For_Firebird()
    {
        var sql = new SqlQuery()
            .Dialect(FirebirdDialect.Instance)
            .Select("c")
            .From("t")
            .Skip(5)
            .ToString();

        Assert.Contains("SKIP 5", sql);
    }

    [Fact]
    public void ToString_Offset_Without_Take_For_Postgres()
    {
        var sql = new SqlQuery()
            .Dialect(PostgresDialect.Instance)
            .Select("c")
            .From("t")
            .Skip(5)
            .ToString();

        Assert.Contains("OFFSET 5", sql);
    }

    [Fact]
    public void ToString_Sql2000_Second_Query_With_Distinct()
    {
        var sql = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .Distinct(true)
            .OrderBy("x")
            .Skip(10)
            .ToString();

        Assert.Contains("SELECT DISTINCT", sql);
    }

    [Fact]
    public void ToString_Sql2000_Second_Query_With_Descending_Multiple_OrderBy()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x DESC")
            .OrderBy("y")
            .OrderBy("z")
            .Skip(10);

        var sql = query.ToString();

        Assert.Contains("DECLARE @Value2 SQL_VARIANT", sql);
        Assert.Contains("@Value0 = x", sql);
        Assert.Contains("x < @Value0", sql);
        Assert.Contains("x = @Value0", sql);
    }

    [Fact]
    public void ToString_Oracle_RowNumber_With_Multiple_OrderBy()
    {
        var sql = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .OrderBy("y")
            .Take(10)
            .Skip(5)
            .ToString();

        Assert.Contains("ROW_NUMBER() OVER (ORDER BY x, y)", sql);
    }

    [Fact]
    public void ToString_CountRecords_With_MultipleResultsets()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t");
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains(";\n", sql);
        Assert.Contains("SELECT count(*)", sql);
    }

    [Fact]
    public void ToString_CountRecords_Without_MultipleResultsets()
    {
        var query = new SqlQuery()
            .Dialect(FirebirdDialect.Instance)
            .Select("c")
            .From("t");
        query.CountRecords = true;

        Assert.Contains("\n---\n", query.ToString());
    }

    [Fact]
    public void ToString_CountRecords_With_Distinct()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t")
            .Distinct(true);
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains("SELECT DISTINCT", sql);
        Assert.Contains(") x__alias__", sql);
    }

    [Fact]
    public void ToString_CountRecords_With_GroupBy()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t")
            .GroupBy("c");
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains("1 as x__alias__x", sql);
        Assert.Contains(") x__alias__", sql);
    }
}
