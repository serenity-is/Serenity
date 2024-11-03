namespace Serenity.Tests.Data;

public class SqlQuery_GetExpression_Tests
{
    [Fact]
    public void GetExpressionThrowsArgumentNullIfColumnNameIsNull()
    {
        var query = new SqlQuery()
            .Select("SomeColumn")
            .From("SomeTable");

        Assert.Throws<ArgumentNullException>(() => ((IGetExpressionByName)query).GetExpression(null));
    }

    [Fact]
    public void GetExpressionReturnsNullIfNoSuchColumn()
    {
        var query = new SqlQuery()
            .Select("SomeColumn")
            .From("SomeTable");

        Assert.Null(((IGetExpressionByName)query).GetExpression("OtherColumn"));
    }

    [Fact]
    public void GetExpressionWorksWithNonAliasedColumns()
    {
        var query = new SqlQuery()
            .Select("SomeColumn")
            .From("SomeTable");

        Assert.Equal(
            "SomeColumn",
            ((IGetExpressionByName)query).GetExpression("SomeColumn"));
    }

    [Fact]
    public void GetExpressionWorksWithAliasedColumns()
    {
        var query = new SqlQuery()
            .Select("SomeColumn", "x")
            .From("SomeTable");

        Assert.Equal(
            "SomeColumn",
            ((IGetExpressionByName)query).GetExpression("x"));
    }

    [Fact]
    public void GetExpressionShouldNotUseOriginalExpressionIfItIsAliased()
    {
        var query = new SqlQuery()
            .Select("SomeColumn", "x")
            .From("SomeTable");

        Assert.Null(((IGetExpressionByName)query).GetExpression("SomeColumn"));
    }

    [Fact]
    public void GetExpressionDoesntWorkWithAliasDotFieldnameAndNoColumnName()
    {
        // not sure if we could make a workaround for this case (e.g. parse expression and find expected column name?)
        var query = new SqlQuery()
            .Select(new Alias("x"), "SomeColumn")
            .From("SomeTable");

        Assert.Null(((IGetExpressionByName)query).GetExpression("SomeColumn"));
    }

    [Fact]
    public void GetExpressionWithIndexWorks()
    {
        var query = new SqlQuery()
            .Select(new Alias("x"), "SomeColumn", "c1")
            .Select("a.b", "c2")
            .Select("c", "c3")
            .From("SomeTable");

        Assert.Equal("x.SomeColumn", ((ISqlQueryExtensible)query).Columns[0].Expression);
        Assert.Equal("a.b", ((ISqlQueryExtensible)query).Columns[1].Expression);
        Assert.Equal("c", ((ISqlQueryExtensible)query).Columns[2].Expression);
    }

    [Fact]
    public void GetExpressionsWithOutOfBoundsIndexThrowsArgumentOutOfRange()
    {
        var query = new SqlQuery().Select("a").Select("b");
        Assert.Throws<ArgumentOutOfRangeException>(() => ((ISqlQueryExtensible)query).Columns[2].Expression);
        Assert.Throws<ArgumentOutOfRangeException>(() => ((ISqlQueryExtensible)query).Columns[-1].Expression);
    }

    [Fact]
    public void GetExpressionWorksWithAliasDotFieldnameAndColumnName()
    {
        // not sure if we could make a workaround for these
        var query = new SqlQuery()
            .Select(new Alias("x"), "SomeColumn", "SomeColumn")
            .From("SomeTable");

        Assert.Equal("x.SomeColumn", ((IGetExpressionByName)query).GetExpression("SomeColumn"));
    }

    [Fact]
    public void GetExpressionReturnsColumnNameIfNoExpression()
    {
        var query = new SqlQuery()
            .Select("SomeColumn");

        Assert.Equal("SomeColumn", ((IGetExpressionByName)query).GetExpression("SomeColumn"));
    }
}