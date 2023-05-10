namespace Serenity.Tests.Data;

public class SqlQuery_Union_Tests
{
    [Fact]
    public void UnionWorksProperly()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .From("T")
            .Select("A")
            .Select("B")
            .Union()
            .From("X")
            .Select("U", "A")
            .Select("W", "B")
            .OrderBy("A");

        Assert.Equal(
            Normalize.Sql(
                "SELECT A, B FROM T UNION SELECT U AS [A], W AS [B] FROM X ORDER BY A"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void UnionClearsSkipTake()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .From("T")
            .Skip(4)
            .Take(3)
            .Select("A")
            .Select("B")
            .OrderBy("C")
            .Union()
            .From("X")
            .Select("U", "A")
            .Select("W", "B")
            .OrderBy("A");

        Assert.Equal(
            Normalize.Sql(
                "SELECT A, B FROM T ORDER BY C OFFSET 4 ROWS FETCH NEXT 3 ROWS ONLY UNION SELECT U AS [A], W AS [B] FROM X ORDER BY A"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void UnionIntersectWorksProperly()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .From("T")
            .Select("A")
            .Select("B")
            .Union(SqlUnionType.Intersect)
            .From("X")
            .Select("U", "A")
            .Select("W", "B")
            .OrderBy("A");

        Assert.Equal(
            Normalize.Sql(
                "SELECT A, B FROM T INTERSECT SELECT U AS [A], W AS [B] FROM X ORDER BY A"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void UnionExceptWorksProperly()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .From("T")
            .Select("A")
            .Select("B")
            .Union(SqlUnionType.Except)
            .From("X")
            .Select("U", "A")
            .Select("W", "B")
            .OrderBy("A");

        Assert.Equal(
            Normalize.Sql(
                "SELECT A, B FROM T EXCEPT SELECT U AS [A], W AS [B] FROM X ORDER BY A"),
            Normalize.Sql(query.ToString()));
    }
}