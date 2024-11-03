namespace Serenity.Tests.Data;

public class SqlQuery_Where_Tests
{
    [Fact]
    public void WhereDoesAndWhenCalledMoreThanOnce()
    {
        var query = new SqlQuery().From("t").Select("c").Where("x > 5").Where("y < 4");
        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t WHERE x > 5 AND y < 4"),
            Normalize.Sql(
                query.ToString())
        );
    }

    [Fact]
    public void WhereWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where((string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Where(String.Empty));
    }
}