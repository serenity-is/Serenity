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
        var query = new SqlQuery().FullTextSearchJoin(
            "Table", "Field1", "word", "T0", "ID", "CT");

        var sql = query.ToString();
        Assert.Contains("INNER JOIN CONTAINSTABLE(", sql);
        Assert.Contains("CT.[key] = T0.ID", sql);
    }

    [Fact]
    public void FullTextSearchJoin_Escapes_Quotes_And_Adds_Newline_When_From_Exists()
    {
        var query = new SqlQuery()
            .From("Parent")
            .FullTextSearchJoin("Table", "Field1", "o'brien", "T0", "ID", "CT");

        var sql = query.ToString();
        Assert.Contains("o''brien", sql);
        Assert.Contains("Parent", sql);
    }
}
