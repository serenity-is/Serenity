namespace Serenity.Tests.Data;

public class SqlQuery_From_Tests
{
    [Fact]
    public void FromMultipleCallsDoesCrossJoin()
    {
        var query = new SqlQuery()
            .From("TestTable1")
            .From("TestTable2")
            .Select("TestColumn");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable1, TestTable2"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void FromMixedOverloadsMultipleCallsDoesCrossJoin()
    {
        var query = new SqlQuery()
            .From("TestTable1", new Alias("x1"))
            .From(new Alias("TestTable2", "x2"))
            .From("TestTable3")
            .Select("TestColumn");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable1 x1, TestTable2 x2, TestTable3"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void FromWithNullOrEmptyArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From(""));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((Alias)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((string)null, (Alias)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From("", (Alias)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From("x", (Alias)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From((ISqlQuery)null, new Alias("x")));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().From(new SqlQuery(), (Alias)null));
    }

    [Fact]
    public void FromWithAliasWithoutTableNameThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(delegate
        {
            new SqlQuery().From(new Alias("TestAlias"));
        });
    }

    [Fact]
    public void FromWithTableNameAndAliasWorks()
    {
        var query = new SqlQuery()
            .From("TestTable", new Alias("TestAlias"))
            .Select("TestColumn");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable TestAlias"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void FromThrowsArgumentOutOfRangeIfSameAliasUsedTwice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(delegate
        {
            new SqlQuery()
                .From("TestTable", new Alias("x"))
                .From("AnotherTable", new Alias("x"));
        });
    }

    [Fact]
    public void FromWithOnlyAliasWorks()
    {
        var query = new SqlQuery()
            .From(new Alias("TestTable", "TestAlias"))
            .Select("TestColumn");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable TestAlias"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void FromWithSubQueryAndAliasWorks()
    {
        var query = new SqlQuery().With(me => me
            .From(me.SubQuery()
                .From("SubTable")
                .Select("SubColumn"), new Alias("sub"))
            .Select("SubColumn"));

        Assert.Equal(
            Normalize.Sql(
                "SELECT SubColumn FROM (SELECT SubColumn FROM SubTable) sub"),
            Normalize.Sql(
                query.ToString()));
    }
}