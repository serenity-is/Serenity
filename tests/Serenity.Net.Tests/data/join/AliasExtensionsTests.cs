namespace Serenity.Data;

public class AliasExtensionsTests
{
    [Fact]
    public void WithTableHint_Keeps_Name_Clean_For_Column_References()
    {
        var alias = new Alias("Tbl", "t").WithTableHint("NOLOCK");

        Assert.Equal("t", alias.Name);
        Assert.Equal("t.", alias.NameDot);
        Assert.Equal("t.C", alias["C"]);
        Assert.Equal("t.C", alias + "C");
        Assert.DoesNotContain("WITH", alias.NameDot);
    }

    [Fact]
    public void WithTableHint_Requires_Alias_And_Hint()
    {
        Assert.Throws<ArgumentNullException>(() => ((IAlias)null).WithTableHint("NOLOCK"));
        Assert.Throws<ArgumentException>(() => new Alias("t").WithTableHint());
        Assert.Throws<ArgumentException>(() => new Alias("t").WithTableHint("  "));
        Assert.Throws<ArgumentNullException>(() => ((IAlias)null).WithNoLock());
    }

    [Fact]
    public void From_WithTableHint_Renders_Hint_Only_In_From()
    {
        var t = new Alias("Tbl", "t").WithTableHint("NOLOCK");
        var query = new SqlQuery().From(t).Select(t, "C");

        var sql = query.ToString();
        Assert.Contains("t.C", sql);
        Assert.Contains("WITH(NOLOCK)", sql);
        Assert.DoesNotContain("WITH(NOLOCK).", sql);
    }

    [Fact]
    public void LeftJoin_WithNoLock_Renders_Hint_Only_In_Join()
    {
        var t2 = new Alias("Table2", "t2").WithNoLock();
        var query = new SqlQuery().From("T0")
            .LeftJoin("Table2", t2, new Criteria("t2", "X") == new Criteria("T0", "Y"))
            .Select(t2, "Z");

        var sql = query.ToString();
        Assert.Contains("t2.[X]", sql);
        Assert.Contains("t2.Z", sql);
        Assert.Contains("WITH(NOLOCK)", sql);
        Assert.DoesNotContain("WITH(NOLOCK).", sql);
    }

    [Fact]
    public void WithTableHint_Supports_Multiple_Hints()
    {
        var t = new Alias("Tbl", "t").WithTableHint("READPAST", "UPDLOCK");
        var query = new SqlQuery().From(t);

        Assert.Contains("WITH(READPAST, UPDLOCK)", query.ToString());
    }

    [Fact]
    public void WithTableHint_Accumulates_Hints_From_Multiple_Calls()
    {
        var t = new Alias("Tbl", "t")
            .WithTableHint("NOLOCK")
            .WithTableHint("READPAST", "UPDLOCK")
            .WithTableHint("nolock", "INDEX(MyIndex)");
        var query = new SqlQuery().From(t);

        Assert.Contains("WITH(NOLOCK, READPAST, UPDLOCK, INDEX(MyIndex))", query.ToString());
    }

    [Fact]
    public void WithNoLock_IsIdempotent()
    {
        var t = new Alias("Tbl", "t").WithNoLock().WithNoLock();
        var query = new SqlQuery().From(t).Select(t, "C");

        Assert.Single(System.Text.RegularExpressions.Regex.Matches(
            query.ToString(), "WITH\\(NOLOCK\\)").Cast<System.Text.RegularExpressions.Match>());
    }

    [Fact]
    public void LeftJoin_WithTableHint_Registers_Clean_Alias_And_Deduplicates()
    {
        var t = new Alias("Table2", "t2").WithNoLock();
        var on = new Criteria("t2", "X") == new Criteria("T0", "Y");
        var query = new SqlQuery().From("T0")
            .LeftJoin(t, on);

        Assert.True(query.HasAlias("t2"));

        query.LeftJoin(t, on);

        var sql = query.ToString();
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(
            sql, "WITH\\(NOLOCK\\)").Cast<System.Text.RegularExpressions.Match>());
    }
}
