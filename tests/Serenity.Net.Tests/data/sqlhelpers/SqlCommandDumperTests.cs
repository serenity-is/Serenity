using Microsoft.Data.SqlClient;

namespace Serenity.Data;

public class SqlCommandDumperTests
{
    private static MockDbCommand GetCommand() => new(null);

    private static SqlParameter Param(string name, string prefix, object value, SqlDbType dbType, ParameterDirection direction = ParameterDirection.Input)
    {
        return new SqlParameter
        {
            ParameterName = prefix + name,
            DbValue = value,
            SqlDbType = dbType,
            Direction = direction
        };
    }

    [Fact]
    public void GetCommandText_TextCommand_WithoutParameters()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "SELECT 1";

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Equal("SELECT 1", result.Trim());
    }

    [Fact]
    public void GetCommandText_DeclarationsForVariousParameterValues()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "SELECT 1";
        command.CommandTimeout = 30;
        command.CommandType = CommandType.Text;

        command.Parameters.Add(new SqlParameter { ParameterName = "@s", DbValue = "test'ed", SqlDbType = SqlDbType.NVarChar, Size = 50, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@i", DbValue = 5, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@b", DbValue = true, SqlDbType = SqlDbType.Bit, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@dt", DbValue = new DateTime(2023, 5, 1), SqlDbType = SqlDbType.DateTime, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@dto", DbValue = new DateTimeOffset(2023, 5, 1, 12, 0, 0, TimeSpan.Zero), SqlDbType = SqlDbType.DateTimeOffset, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@g", DbValue = Guid.NewGuid(), SqlDbType = SqlDbType.UniqueIdentifier, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@n", DbValue = 1.5m, SqlDbType = SqlDbType.Decimal, Direction = ParameterDirection.Input });
        command.Parameters.Add(new SqlParameter { ParameterName = "@null", DbValue = DBNull.Value, SqlDbType = SqlDbType.VarChar, Size = 10, Direction = ParameterDirection.Input });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("DECLARE @s NVARCHAR(50) = 'test''ed';", result, StringComparison.Ordinal);
        Assert.Contains("DECLARE @i INT = 5;", result, StringComparison.Ordinal);
        Assert.Contains("DECLARE @b BIT = 1;", result, StringComparison.Ordinal);
        Assert.Contains("DECLARE @dt DATETIME = " + new DateTime(2023, 5, 1).ToSql(SqlServer2012Dialect.Instance), result, StringComparison.Ordinal);
        Assert.Contains("@g UNIQUEIDENTIFIER", result, StringComparison.Ordinal);
        Assert.Contains("@n DECIMAL = 1.5", result, StringComparison.Ordinal);
        Assert.Contains("@null VARCHAR(10) = NULL", result, StringComparison.Ordinal);
        Assert.Contains("SELECT 1", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_PlainDbParameterDbType_LogsDbType()
    {
        var command = GetCommand();
        command.CommandText = "SELECT 1";
        var parameter = new MockDbParameter { DbType = DbType.String, ParameterName = "@p1" };
        ((MockDbParameterCollection)command.Parameters).Add(parameter);

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("DECLARE @p1 String", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_StoredProcedure_WithReturnValueAndOutput()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "MyProc";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter { ParameterName = "@p1", DbValue = "abc", SqlDbType = SqlDbType.NVarChar, Size = 10, Direction = ParameterDirection.InputOutput });
        command.Parameters.Add(new SqlParameter { ParameterName = "@p2", DbValue = null, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });
        command.Parameters.Add(new SqlParameter { ParameterName = "@p3", DbValue = null, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.ReturnValue });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("DECLARE @returnValue INT;", result, StringComparison.Ordinal);
        Assert.Contains("EXEC @returnValue = MyProc", result, StringComparison.Ordinal);
        Assert.Contains("@p1 = @p1 OUTPUT, @p2 = @p2 OUTPUT", result, StringComparison.Ordinal);
        Assert.Contains("-- RESULTS", result, StringComparison.Ordinal);
        Assert.Contains("@p1 as [@p1], @p2 as [@p2], @returnValue as ReturnValue", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_StoredProcedure_WithoutReturnValue()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "MyProc";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter { ParameterName = "@p1", DbValue = 1, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.DoesNotContain("@returnValue = ", result, StringComparison.Ordinal);
        Assert.Contains("EXEC MyProc@p1 = @p1", result, StringComparison.Ordinal);
        Assert.DoesNotContain("-- RESULTS", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_TextCommandWithOutputParameter_ShowsResults()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "UPDATE T SET A = @p1";
        command.CommandType = CommandType.Text;

        command.Parameters.Add(new SqlParameter { ParameterName = "@p1", DbValue = 1, SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("-- RESULTS", result, StringComparison.Ordinal);
        Assert.Contains("@p1 as [@p1];", result, StringComparison.Ordinal);
        Assert.Contains("UPDATE T SET A = @p1", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_UnknownSqlDbType_WritesUnknownComments()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "SELECT 1";

        command.Parameters.Add(new SqlParameter { ParameterName = "@ts", DbValue = null, SqlDbType = SqlDbType.Timestamp, Direction = ParameterDirection.Input });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("UNKNOWN DATATYPE: TIMESTAMP", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetCommandText_UnsupportedParameterValueType_WritesUnknownComment()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "SELECT 1";

        command.Parameters.Add(new SqlParameter { ParameterName = "@o", DbValue = new SqlCommandDumperTests(), SqlDbType = SqlDbType.NVarChar, Size = 10, Direction = ParameterDirection.Input });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("UNKNOWN DATATYPE", result, StringComparison.Ordinal);
    }

    [Fact]
    public void GetCommandText_ExceptionInParameterValue_WritesExceptionComment()
    {
        MockDbCommand command = GetCommand();
        command.CommandText = "SELECT 1";

        command.Parameters.Add(new SqlParameter { ParameterName = "@hex", DbValue = new byte[] { 1, 2 }, SqlDbType = SqlDbType.VarBinary, Size = 4, Direction = ParameterDirection.Input });

        var result = SqlCommandDumper.GetCommandText(command);

        Assert.Contains("Exception occurred while converting parameter", result, StringComparison.Ordinal);
        Assert.Contains("FormatException", result, StringComparison.Ordinal);
    }

    public override string ToString() => "SqlCommandDumperTestsCustomValue";
}

