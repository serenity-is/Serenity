namespace Serenity.Tests.Data;

using Concat = ConcatAttribute;

public class ConcatAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Expression1_Or_Expression2_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Concat(null, null));
        Assert.Throws<ArgumentNullException>(() => new Concat(null, "TEST"));
        Assert.Throws<ArgumentNullException>(() => new Concat("TEST", null));
    }

    [Fact]
    public void Throws_ArgumentNull_If_One_Of_Rest_Expressions_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new Concat("TEST", "TEST", (string)null));
        Assert.Throws<ArgumentNullException>(() => new Concat("TEST", "TEST", null)); // passing rest[] as null!
        Assert.Throws<ArgumentNullException>(() => new Concat("TEST", "TEST", "TEST", null));
        Assert.Throws<ArgumentNullException>(() => new Concat("TEST", "TEST", "TEST", "TEST", null));
    }

    [Fact]
    public void NullAsEmpty_IsTrue_ByDefault()
    {
        Assert.True(new Concat("TEST", "TEST").NullAsEmpty);
    }

    [InlineData(typeof(SqlServer2012Dialect), false)]
    [InlineData(typeof(SqlServer2012Dialect), true)]
    [InlineData(typeof(PostgresDialect), false)]
    [InlineData(typeof(PostgresDialect), true)]
    [Theory]
    public void Uses_Concat_For_SqlServer_And_Postgres(Type dialectType, bool nullAsEmpty)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("CONCAT(Test1, Test2)", new Concat("Test1", "Test2") { NullAsEmpty = nullAsEmpty }
            .ToString(dialect));

        Assert.Equal("CONCAT(Test1, Test2, Test3)", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = nullAsEmpty }
            .ToString(dialect));

        Assert.Equal("CONCAT(Test1, Test2, Test3, Test4)", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = nullAsEmpty }
            .ToString(dialect));
    }

    [Fact]
    public void Uses_TwoParam_Concat_For_Oracle()
    {
        Assert.Equal("COALESCE(CONCAT(Test1, Test2), '')", new Concat("Test1", "Test2")
            .ToString(OracleDialect.Instance));

        Assert.Equal("CONCAT(COALESCE(CONCAT(Test1, Test2), ''), Test3)", new Concat("Test1", "Test2", "Test3")
            .ToString(OracleDialect.Instance));

        Assert.Equal("CONCAT(CONCAT(COALESCE(CONCAT(Test1, Test2), ''), Test3), Test4)", 
            new Concat("Test1", "Test2", "Test3", "Test4")
            .ToString(OracleDialect.Instance));
    }

    [Fact]
    public void DoesNotUse_Coalesce_For_Oracle_When_NullAsEmpty_IsFalse()
    {
        Assert.Equal("CONCAT(Test1, Test2)", 
            new Concat("Test1", "Test2") { NullAsEmpty = false }
            .ToString(OracleDialect.Instance));

        Assert.Equal("CONCAT(CONCAT(Test1, Test2), Test3)", 
            new Concat("Test1", "Test2", "Test3") { NullAsEmpty = false }
            .ToString(OracleDialect.Instance));

        Assert.Equal("CONCAT(CONCAT(CONCAT(Test1, Test2), Test3), Test4)", 
            new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = false }
            .ToString(OracleDialect.Instance));
    }

    [InlineData(typeof(MySqlDialect))]
    [Theory]
    public void Uses_MultiParam_Concat_ByDefault_NullAsEmpty_IsTrue(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("CONCAT(COALESCE(Test1, ''), COALESCE(Test2, ''))", 
            new Concat("Test1", "Test2") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("CONCAT(COALESCE(Test1, ''), COALESCE(Test2, ''), COALESCE(Test3, ''))", 
            new Concat("Test1", "Test2", "Test3") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("CONCAT(COALESCE(Test1, ''), COALESCE(Test2, ''), COALESCE(Test3, ''), COALESCE(Test4, ''))", 
            new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = true }
            .ToString(dialect));
    }

    [InlineData(typeof(MySqlDialect))]
    [Theory]
    public void Uses_MultiParam_Concat_ByDefault_NullAsEmpty_IsFalse(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("CONCAT(Test1, Test2)", new Concat("Test1", "Test2") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("CONCAT(Test1, Test2, Test3)", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("CONCAT(Test1, Test2, Test3, Test4)", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = false }
            .ToString(dialect));
    }

    [InlineData(typeof(SqlServer2000Dialect))]
    [InlineData(typeof(SqlServer2005Dialect))]
    [InlineData(typeof(SqlServer2008Dialect))]
    [Theory]
    public void Uses_Plus_Operator_For_Sql_2008_And_Before_NullAsEmpty_IsFalse(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("(Test1 + Test2)", new Concat("Test1", "Test2") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("(Test1 + Test2 + Test3)", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("(Test1 + Test2 + Test3 + Test4)", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = false }
            .ToString(dialect));
    }

    [InlineData(typeof(SqlServer2000Dialect))]
    [InlineData(typeof(SqlServer2005Dialect))]
    [InlineData(typeof(SqlServer2008Dialect))]
    [Theory]
    public void Uses_Plus_Operator_For_Sql_2008_And_Before_NullAsEmpty_IsTrue(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("(COALESCE(Test1, '') + COALESCE(Test2, ''))", new Concat("Test1", "Test2") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("(COALESCE(Test1, '') + COALESCE(Test2, '') + COALESCE(Test3, ''))", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("(COALESCE(Test1, '') + COALESCE(Test2, '') + COALESCE(Test3, '') + COALESCE(Test4, ''))", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = true }
            .ToString(dialect));
    }

    [InlineData(typeof(SqliteDialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Uses_Concentation_Operator_For_ServerTypes_Not_Supporting_Concat_NullAsEmpty_IsFalse(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("(Test1 || Test2)", new Concat("Test1", "Test2") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("(Test1 || Test2 || Test3)", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = false }
            .ToString(dialect));

        Assert.Equal("(Test1 || Test2 || Test3 || Test4)", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = false }
            .ToString(dialect));
    }

    [InlineData(typeof(SqliteDialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Uses_Concentation_Operator_For_ServerTypes_Not_Supporting_Concat_NullAsEmpty_IsTrue(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("(COALESCE(Test1, '') || COALESCE(Test2, ''))", new Concat("Test1", "Test2") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("(COALESCE(Test1, '') || COALESCE(Test2, '') || COALESCE(Test3, ''))", new Concat("Test1", "Test2", "Test3") { NullAsEmpty = true }
            .ToString(dialect));

        Assert.Equal("(COALESCE(Test1, '') || COALESCE(Test2, '') || COALESCE(Test3, '') || COALESCE(Test4, ''))", new Concat("Test1", "Test2", "Test3", "Test4") { NullAsEmpty = true }
            .ToString(dialect));
    }
}