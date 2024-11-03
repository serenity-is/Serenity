namespace Serenity.Tests.Data;

public class SqlQuery_Distinct_Tests
{
    [Fact]
    public void DistinctAddsKeyword()
    {
        var query = new SqlQuery()
            .Distinct(true)
            .Select("TestColumn")
            .From("TestTable");

        Assert.Equal(
            Normalize.Sql(
                "SELECT DISTINCT TestColumn FROM TestTable"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void DistinctCanBeTurnedOff()
    {
        var query = new SqlQuery()
            .Distinct(true)
            .Select("TestColumn")
            .From("TestTable")
            .Distinct(false);

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable"),
            Normalize.Sql(
                query.ToString()));
    }
}