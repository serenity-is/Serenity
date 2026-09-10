namespace Serenity.Data;

public class SqlQuery_MiscTests
{
    [Fact]
    public void Select_SubQuery_With_ColumnName_And_Guards()
    {
        var sub = new SqlQuery().Select("1");

        var query = new SqlQuery().Select(sub, "x");

        Assert.Contains("1 AS [x]", query.ToString());
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null!, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select(sub, ""));
    }

    [Fact]
    public void Select_SubQuery_Without_ColumnName_And_Guards()
    {
        var sub = new SqlQuery().Select("1");

        var query = new SqlQuery().Select(sub);

        Assert.Contains("1", query.ToString());
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Select((ISqlQuery)null!));
    }

    [Fact]
    public void SelectMany_Selects_All_Expressions()
    {
        var query = new SqlQuery().SelectMany("a", "b", "c");
        var sql = query.ToString();

        Assert.Contains("a", sql);
        Assert.Contains("b", sql);
        Assert.Contains("c", sql);
    }

    [Fact]
    public void Text_Returns_ToString()
    {
        var query = new SqlQuery().Select("c").From("t");
        Assert.Equal(query.ToString(), query.Text);
    }

    [Fact]
    public void OmitParens_Sets_Flag()
    {
        var query = new SqlQuery();

        Assert.Same(query, query.OmitParens());
        Assert.Same(query, query.OmitParens(true));
        Assert.Same(query, query.OmitParens(false));
    }

    [Fact]
    public void GroupBy_Validates_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy((string)null!));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(""));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(new Alias("T", "A"), ""));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().GroupBy(null!, "x"));
    }
}
