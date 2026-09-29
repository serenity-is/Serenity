using System.Data;
using System.Data.Common;

namespace Serenity.Data;

public class SqlHelperExecuteReaderTests
{
    private static (MockDbConnection connection, Func<MockDbCommand?> lastCommand) Create(
        Func<MockDbCommand, DbDataReader> onReader)
    {
        MockDbCommand? command = null;
        var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(cmd =>
            {
                command = cmd;
                return onReader(cmd);
            });
        return (connection, () => command);
    }

    [Fact]
    public void ExecuteReader_Disposes_Command_When_Reader_Disposed()
    {
        var (connection, lastCommand) = Create(_ => new MockDbDataReader(
            new { Id = 1 }));

        using (IDataReader reader = SqlHelper.ExecuteReader(connection, "SELECT 1", null))
        {
            Assert.IsAssignableFrom<System.Data.Common.DbDataReader>(reader);
            Assert.True(reader.Read());
            Assert.Equal(1, reader["Id"]);
            Assert.Equal(0, lastCommand()!.DisposeCalls);
        }

        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }

    [Fact]
    public void ExecuteReader_Disposes_Command_When_Reader_Closed()
    {
        var (connection, lastCommand) = Create(_ => new MockDbDataReader());

        var reader = SqlHelper.ExecuteReader(connection, "SELECT 1", null);
        reader.Close();

        Assert.Equal(1, lastCommand()!.DisposeCalls);
        reader.Dispose();
        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }

    [Fact]
    public void ExecuteReader_Disposes_Command_When_Execute_Throws()
    {
        var (connection, lastCommand) = Create(_ => throw new InvalidOperationException("boom"));

        var ex = Assert.Throws<InvalidOperationException>(
            () => SqlHelper.ExecuteReader(connection, "SELECT 1", null));

        Assert.Equal("boom", ex.Message);
        Assert.Equal("SELECT 1", ex.Data["sql_command_text"]);
        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }

    [Fact]
    public async Task ExecuteReaderAsync_Disposes_Command_When_Reader_Disposed()
    {
        var (connection, lastCommand) = Create(_ => new MockDbDataReader(
            new { Id = 1 }));

        using (IDataReader reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT 1", null))
        {
            Assert.IsAssignableFrom<System.Data.Common.DbDataReader>(reader);
            Assert.True(reader.Read());
            Assert.Equal(0, lastCommand()!.DisposeCalls);
        }

        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }

    [Fact]
    public void ExecuteReader_Delegates_All_Members_To_Inner_Reader()
    {
        var (connection, _) = Create(_ => new MockDbDataReader(
            new { Id = 7, Name = "x" },
            new { Id = 8, Name = (string?)null }));

        using IDataReader reader = SqlHelper.ExecuteReader(connection, "SELECT 1", null);

        Assert.Equal(0, reader.Depth);
        Assert.Equal(2, reader.FieldCount);
        Assert.Equal(2, reader.RecordsAffected);
        Assert.False(reader.IsClosed);
        Assert.Equal("Id", reader.GetName(0));
        Assert.Equal(1, reader.GetOrdinal("Name"));
        Assert.Equal(typeof(int), reader.GetFieldType(0));

        Assert.True(reader.Read());
        Assert.Equal(7, reader.GetInt32(0));
        Assert.Equal("x", reader.GetString(1));
        Assert.Equal(7, reader[0]);
        Assert.Equal("x", reader["Name"]);
        Assert.False(reader.IsDBNull(0));

        // MockDbDataReader.GetValues throws like HasRows: pass-through, not masking.
        Assert.Throws<NotImplementedException>(() => { var ignored = reader.GetValues(new object[2]); });

        Assert.True(reader.Read());
        Assert.True(reader.IsDBNull(1));

        // MockDbDataReader.HasRows throws: the wrapper must pass it through,
        // not mask it with a hardcoded value.
        var dbReader = Assert.IsAssignableFrom<DbDataReader>(reader);
        Assert.Throws<NotImplementedException>(() => { var ignored = dbReader.HasRows; });

        // Mock enumerates raw object[] rows; wrapper must expose the same enumerator.
        var count = 0;
        foreach (var row in dbReader)
            count++;
        Assert.Equal(2, count);

        // MockDbDataReader.GetSchemaTable throws: pass-through, not masking.
        Assert.Throws<NotImplementedException>(() => { var ignored = reader.GetSchemaTable(); });
    }

    [Fact]
    public async Task ExecuteReader_Disposes_Command_On_DisposeAsync_Without_Double_Dispose()
    {
        var (connection, lastCommand) = Create(_ => new MockDbDataReader());

        var reader = SqlHelper.ExecuteReader(connection, "SELECT 1", null);
        await ((IAsyncDisposable)reader).DisposeAsync();

        Assert.Equal(1, lastCommand()!.DisposeCalls);
        reader.Dispose();
        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }

    [Fact]
    public async Task ExecuteReaderAsync_Disposes_Command_When_Execute_Throws()
    {
        var (connection, lastCommand) = Create(_ => throw new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => SqlHelper.ExecuteReaderAsync(connection, "SELECT 1", null));

        Assert.Equal("boom", ex.Message);
        Assert.Equal(1, lastCommand()!.DisposeCalls);
    }
}
