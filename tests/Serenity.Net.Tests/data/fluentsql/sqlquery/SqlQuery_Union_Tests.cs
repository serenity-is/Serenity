namespace Serenity.Data;

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
                "SELECT A, B FROM [T] UNION SELECT U AS [A], W AS [B] FROM [X] ORDER BY A"),
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
                "SELECT A, B FROM [T] ORDER BY C OFFSET 4 ROWS FETCH NEXT 3 ROWS ONLY UNION SELECT U AS [A], W AS [B] FROM [X] ORDER BY A"),
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
                "SELECT A, B FROM [T] INTERSECT SELECT U AS [A], W AS [B] FROM [X] ORDER BY A"),
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
                "SELECT A, B FROM [T] EXCEPT SELECT U AS [A], W AS [B] FROM [X] ORDER BY A"),
            Normalize.Sql(query.ToString()));
    }

    [Fact]
    public void UnionTake_Applies_To_Current_Select()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("A").From("T")
            .Union()
            .Select("A").From("X")
            .OrderBy("A")
            .Take(5);

        var sql = Normalize.Sql(query.ToString());
        Assert.Equal(Normalize.Sql(
            "SELECT A FROM [T] UNION SELECT TOP 5 A FROM [X] ORDER BY A"), sql);
    }

    [Fact]
    public void Take_Before_And_After_Union_Applies_To_Respective_Selects()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("A").From("T")
            .OrderBy("A")
            .Take(3)
            .Union()
            .Select("A").From("X")
            .OrderBy("A")
            .Take(5);

        var sql = Normalize.Sql(query.ToString());
        Assert.Equal(Normalize.Sql(
            "SELECT TOP 3 A FROM [T] ORDER BY A UNION SELECT TOP 5 A FROM [X] ORDER BY A"), sql);
    }

    [Fact]
    public void Union_Subquery_Can_Be_Paged_And_Count_From_Outer_Query()
    {
        var mainQuery = new SqlQuery().Dialect(SqlServer2012Dialect.Instance);
        var union = mainQuery.SubQuery()
            .From("T1").Select("A").Take(5)
            .Union()
            .From("T2").Select("A").Take(3);

        mainQuery.From(union, new Alias("u"))
            .Select("u.A")
            .OrderBy("u.A")
            .Skip(5)
            .Take(10);
        mainQuery.CountRecords = true;

        Assert.Equal(Normalize.Sql(
            "SELECT u.A FROM (SELECT TOP 5 A FROM [T1] UNION SELECT TOP 3 A FROM [T2]) u ORDER BY u.A OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY; " +
            "SELECT count(*) FROM (SELECT TOP 5 A FROM [T1] UNION SELECT TOP 3 A FROM [T2]) u"),
            Normalize.Sql(mainQuery.ToString()));
    }

}
