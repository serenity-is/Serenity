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
    public void HasRows_For_Plain_DataReader_Preserves_The_First_Row()
    {
        var reader = new PlainDataReader(hasRow: true);
        using var wrapper = new CommandOwningDataReader(reader, new MockDbCommand());

        Assert.True(wrapper.HasRows);
        Assert.Equal(1, reader.ReadCalls);
        Assert.True(wrapper.Read());
        Assert.Equal(1, reader.ReadCalls);
        Assert.False(wrapper.Read());
        Assert.True(wrapper.HasRows);
    }

    [Fact]
    public void HasRows_For_Empty_Plain_DataReader_Is_False()
    {
        var reader = new PlainDataReader(hasRow: false);
        using var wrapper = new CommandOwningDataReader(reader, new MockDbCommand());

        Assert.False(wrapper.HasRows);
        Assert.False(wrapper.Read());
        Assert.Equal(1, reader.ReadCalls);
    }

    [Fact]
    public async Task DisposeAsync_Calls_Base_Disposal_Even_When_Already_Released()
    {
        var reader = new PlainDataReader(hasRow: false);
        var command = new MockDbCommand();
        var wrapper = new CommandOwningDataReader(reader, command);

        await wrapper.DisposeAsync();
        await wrapper.DisposeAsync();

        Assert.Equal(1, reader.DisposeCalls);
        Assert.Equal(1, command.DisposeCalls);
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

    private sealed class PlainDataReader(bool hasRow) : IDataReader
    {
        private bool rowAvailable = hasRow;

        public int ReadCalls { get; private set; }
        public int DisposeCalls { get; private set; }
        public int Depth => 0;
        public bool IsClosed => false;
        public int RecordsAffected => 0;
        public int FieldCount => 0;
        public object this[int i] => throw new IndexOutOfRangeException();
        public object this[string name] => throw new IndexOutOfRangeException();
        public void Close() { }
        public void Dispose() => DisposeCalls++;
        public bool Read()
        {
            ReadCalls++;
            var result = rowAvailable;
            rowAvailable = false;
            return result;
        }
        public bool NextResult() => false;
        public DataTable? GetSchemaTable() => null;
        public bool GetBoolean(int i) => throw new NotSupportedException();
        public byte GetByte(int i) => throw new NotSupportedException();
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => throw new NotSupportedException();
        public char GetChar(int i) => throw new NotSupportedException();
        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => throw new NotSupportedException();
        public IDataReader GetData(int i) => throw new NotSupportedException();
        public string GetDataTypeName(int i) => throw new NotSupportedException();
        public DateTime GetDateTime(int i) => throw new NotSupportedException();
        public decimal GetDecimal(int i) => throw new NotSupportedException();
        public double GetDouble(int i) => throw new NotSupportedException();
        public System.Collections.IEnumerator GetEnumerator() => throw new NotSupportedException();
        public Type GetFieldType(int i) => throw new NotSupportedException();
        public float GetFloat(int i) => throw new NotSupportedException();
        public Guid GetGuid(int i) => throw new NotSupportedException();
        public short GetInt16(int i) => throw new NotSupportedException();
        public int GetInt32(int i) => throw new NotSupportedException();
        public long GetInt64(int i) => throw new NotSupportedException();
        public string GetName(int i) => throw new NotSupportedException();
        public int GetOrdinal(string name) => throw new NotSupportedException();
        public string GetString(int i) => throw new NotSupportedException();
        public object GetValue(int i) => throw new NotSupportedException();
        public int GetValues(object[] values) => throw new NotSupportedException();
        public bool IsDBNull(int i) => throw new NotSupportedException();
    }
}
