namespace Serenity.Data;

public class SqlHelperAddParamWithValueTests
{
    [Fact]
    public void AddParamWithValue_ConvertsParameterPrefix()
    {
        using var connection = new MockDbConnection { Dialect = OracleDialect.Instance };
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", 5, OracleDialect.Instance);

        Assert.Equal(":p1", param.ParameterName);
        Assert.Same(param, command.Parameters[0]);
    }

    [Fact]
    public void AddParamWithValue_KeepsName_WhenPrefixMatches()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", 5, SqlServer2012Dialect.Instance);

        Assert.Equal("@p1", param.ParameterName);
    }

    [Fact]
    public void AddParamWithValue_KeepsName_WhenDialectPrefixIsAt()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", 5, PostgresDialect.Instance);

        // name is converted only when it starts with @ and dialect prefix differs
        Assert.Equal("@p1", param.ParameterName);
    }

    [Fact]
    public void AddParamWithValue_NullValue_BecomesDBNull()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", null, SqlServer2012Dialect.Instance);

        Assert.Equal(DBNull.Value, param.Value);
    }

    [Fact]
    public void AddParamWithValue_Bool_WithNeedsBoolWorkaround_ConvertsToNumber()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", true, new BoolWorkaroundDialect());

        Assert.Equal(1, param.Value);
    }

    [Fact]
    public void AddParamWithValue_False_WithNeedsBoolWorkaround_ConvertsToZero()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", false, new BoolWorkaroundDialect());

        Assert.Equal(0, param.Value);
    }

    [Fact]
    public void AddParamWithValue_Bool_WithoutWorkaround_KeepsBool()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", true, SqlServer2012Dialect.Instance);

        Assert.Equal(true, param.Value);
    }

    [Fact]
    public void AddParamWithValue_NullSqlBinary_BecomesDBNullWithBinaryType()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1",
            System.Data.SqlTypes.SqlBinary.Null, SqlServer2012Dialect.Instance);

        Assert.Equal(DBNull.Value, param.Value);
        Assert.Equal(DbType.Binary, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_ShortString_SetsSize4000()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", "test", SqlServer2012Dialect.Instance);

        Assert.Equal("test", param.Value);
        Assert.Equal(4000, param.Size);
    }

    [Fact]
    public void AddParamWithValue_LongString_DoesNotSetSize()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new string('x', 4001), SqlServer2012Dialect.Instance);

        Assert.Equal(0, param.Size);
    }

    [Fact]
    public void AddParamWithValue_DateTime_WithUseDateTime2_SetsDateTime2Type()
    {
        // Dapper maps DateTime to null, so the DateTime2 upgrade only happens
        // when the provider parameter is already typed as DateTime (like SqlClient does)
        using var connection = new MockDbConnection { CreatedParameterDbType = DbType.DateTime };
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new DateTime(2024, 1, 15), SqlServer2012Dialect.Instance);

        Assert.Equal(DbType.DateTime2, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_DateTime_WithoutUseDateTime2_KeepsDateTimeType()
    {
        using var connection = new MockDbConnection { CreatedParameterDbType = DbType.DateTime };
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new DateTime(2024, 1, 15), SqlServer2000Dialect.Instance);

        Assert.Equal(DbType.DateTime, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_DateTime_FreshParameter_KeepsProviderDefault()
    {
        // Dapper has no DbType mapping for DateTime, so a fresh parameter keeps its default type
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new DateTime(2024, 1, 15), SqlServer2012Dialect.Instance);

        Assert.Equal(DbType.AnsiString, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_DateOnly_KeepsDateType()
    {
        using var connection = new MockDbConnection { CreatedParameterDbType = DbType.Date };
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new DateOnly(2024, 1, 15), SqlServer2012Dialect.Instance);

        Assert.Equal(DbType.Date, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_DateOnly_FreshParameter_UsesDapperObjectMapping()
    {
        // Dapper maps DateOnly to DbType.Object, so a fresh parameter gets Object
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", new DateOnly(2024, 1, 15), SqlServer2012Dialect.Instance);

        Assert.Equal(DbType.Object, param.DbType);
    }

    [Fact]
    public void AddParamWithValue_EnumValue_IsConvertedToUnderlyingType()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", SampleEnum.First, SqlServer2012Dialect.Instance);

        Assert.Equal(1, param.Value);
    }

    [Fact]
    public void AddParamWithValue_IntValue_UsesMappedDbType()
    {
        using var connection = new MockDbConnection();
        using var command = connection.CreateCommand();

        var param = command.AddParamWithValue("@p1", 5, SqlServer2012Dialect.Instance);

        Assert.Equal(5, param.Value);
        Assert.Equal(DbType.Int32, param.DbType);
    }

    private class BoolWorkaroundDialect : SqlServer2012Dialect
    {
        public override bool NeedsBoolWorkaround => true;
    }
}
