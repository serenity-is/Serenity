namespace Serenity.Data;

public class SqlQuery_FullTextSearchTests
{
    [Fact]
    public void FullTextSearchJoin_Throws_For_Missing_Arguments()
    {
        var query = new SqlQuery();

        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin(null!, "F", "q", "T0", "K", "CT"));
        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin("T", "", "q", "T0", "K", "CT"));
        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin("T", "F", "", "T0", "K", "CT"));
        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin("T", "F", "q", "", "K", "CT"));
        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin("T", "F", "q", "T0", "", "CT"));
        Assert.Throws<ArgumentNullException>(() =>
            query.FullTextSearchJoin("T", "F", "q", "T0", "K", ""));
    }

    [Fact]
    public void FullTextSearchJoin_Appends_Join()
    {
        var query = new SqlQuery().Select("T0.Id").FullTextSearchJoin(
            "Table", "Field1", "word", "T0", "ID", "CT");

        var sql = query.ToString();
        Assert.Contains("INNER JOIN CONTAINSTABLE(", sql);
        Assert.Contains("CT.[key] = T0.ID", sql);
    }

    [Fact]
    public void FullTextSearchJoin_Uses_Param_And_Adds_Newline_When_From_Exists()
    {
        var query = new SqlQuery()
            .From("Parent")
            .Select("Parent.Id")
            .FullTextSearchJoin("Table", "Field1", "o'brien", "T0", "ID", "CT");

        var sql = query.ToString();
        Assert.DoesNotContain("o'brien", sql, StringComparison.Ordinal);
        Assert.Contains("@p1", sql, StringComparison.Ordinal);
        Assert.Equal("o'brien", query.Params!["@p1"]);
        Assert.Contains("Parent", sql);
    }
}
