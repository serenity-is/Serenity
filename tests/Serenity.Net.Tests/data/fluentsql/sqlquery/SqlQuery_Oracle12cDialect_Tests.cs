namespace Serenity.Tests.Data;

public class SqlQuery_Oracle12cDialect_Tests
{
    [Fact]
    public void SkipTakeUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t")
            .Take(20)
            .Skip(50);

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t OFFSET 50 ROWS FETCH NEXT 20 ROWS ONLY"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SkipTakeWithOrderByUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .Take(20)
            .Skip(50);

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t ORDER BY x OFFSET 50 ROWS FETCH NEXT 20 ROWS ONLY"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void TakeUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t")
            .Take(10);

        var a = Normalize.Sql(@"SELECT c FROM t OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void TakeOrderByUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("c")
            .Take(10);

        var a = Normalize.Sql(@"SELECT c FROM t ORDER BY c OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void TakeWhereOrderByUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t")
            .Where("c = 1")
            .OrderBy("c")
            .Take(10);

        var a = Normalize.Sql(@"SELECT c FROM t WHERE c = 1 ORDER BY c OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void WithoutTakeUsesCorrectSyntaxForOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .Select("c")
            .From("t");

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t"),
            Normalize.Sql(
                query.ToString()));

    }

    /// <summary>
    /// In case of Oracle Union with Order By query the name of Order By params 
    /// should match the name of first part of Union clause, alternatively 
    /// we can use the serial number of the selected fields 
    /// Ref: https://docs.oracle.com/database/121/SQLRF/queries004.htm#SQLRF52341, 
    /// https://www.techonthenet.com/oracle/union_all.php
    /// </summary>
    [Fact]
    public void UnionWorksProperlyOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .From("T")
            .Select("A")
            .Select("B")
            .Union()
            .From("X")
            .Select("U")
            .Select("W")
            .OrderBy("A");

        var a = Normalize.Sql(@"SELECT A, B FROM T UNION SELECT U, W FROM X ORDER BY A");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void UnionClearsSkipTakeOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .From("T")
            .Skip(4)
            .Take(3)
            .Select("A", "AliasA")
            .Select("B", "AliasB")
            .OrderBy("C")
            .Union()
            .From("X")
            .Select("U")
            .Select("W")
            .OrderBy("AliasA");

        var a = Normalize.Sql(@"SELECT A AS ""ALIASA"", B AS ""ALIASB"" FROM T ORDER BY C OFFSET 4 ROWS FETCH NEXT 3 ROWS ONLY UNION SELECT U, W FROM X ORDER BY AliasA");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void UnionIntersectWorksProperlyOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .From("T")
            .Select("A", "AliasA")
            .Select("B", "AliasB")
            .Union(SqlUnionType.Intersect)
            .From("X")
            .Select("U")
            .Select("W")
            .OrderBy("1");

        var a = Normalize.Sql(@"SELECT A AS ""ALIASA"", B AS ""ALIASB"" FROM T INTERSECT SELECT U, W FROM X ORDER BY 1");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

    [Fact]
    public void UnionExceptWorksProperlyOracle12c()
    {
        var query = new SqlQuery()
            .Dialect(Oracle12cDialect.Instance)
            .From("T")
            .Select("A", "AliasA")
            .Select("B", "AliasB")
            .Union(SqlUnionType.Except)
            .From("X")
            .Select("U")
            .Select("W")
            .OrderBy("2");

        var a = Normalize.Sql(@"SELECT A AS ""ALIASA"", B AS ""ALIASB"" FROM T MINUS SELECT U, W FROM X ORDER BY 2");
        var b = Normalize.Sql(query.ToString());
        Assert.Equal(a, b);
    }

}