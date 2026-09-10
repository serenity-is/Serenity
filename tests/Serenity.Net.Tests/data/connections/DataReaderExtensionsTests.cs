namespace Serenity.Data;

public class DataReaderExtensionsTests
{
    private static MockDbDataReader Reader(string field, object value)
    {
        var reader = new MockDbDataReader(
            [new Dictionary<string, object> { [field] = value }],
            field);
        reader.Read();
        return reader;
    }

    // As methods (typed getters)

    [Fact]
    public void AsDateTimeDbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsDateTime(0));

    [Fact]
    public void AsDateTime_Value_ReturnsValue() => Assert.Equal(new DateTime(2023, 5, 1, 10, 0, 0), Reader("F", new DateTime(2023, 5, 1, 10, 0, 0)).AsDateTime(0));

    [Fact]
    public void ToDateTime_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToDateTime(0));

    [Fact]
    public void ToDateTime_StringValue_Converts() => Assert.Equal(new DateTime(2023, 5, 1, 10, 0, 0), Reader("F", new DateTime(2023, 5, 1, 10, 0, 0)).ToDateTime(0));

    [Fact]
    public void AsDecimal_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsDecimal(0));

    [Fact]
    public void AsDecimal_Value_ReturnsValue() => Assert.Equal(1.5m, Reader("F", 1.5m).AsDecimal(0));

    [Fact]
    public void ToDecimal_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToDecimal(0));

    [Fact]
    public void ToDecimal_Value_Converts() => Assert.Equal(1.5m, Reader("F", 1.5m).ToDecimal(0));

    [Fact]
    public void AsDouble_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsDouble(0));

    [Fact]
    public void AsDouble_Value_ReturnsValue() => Assert.Equal(1.5, Reader("F", 1.5).AsDouble(0));

    [Fact]
    public void ToDouble_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToDouble(0));

    [Fact]
    public void ToDouble_Value_Converts() => Assert.Equal(1.5, Reader("F", 1.5).ToDouble(0));

    [Fact]
    public void AsInt32_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsInt32(0));

    [Fact]
    public void AsInt32_Value_ReturnsValue() => Assert.Equal(5, Reader("F", 5).AsInt32(0));

    [Fact]
    public void ToInt32_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToInt32(0));

    [Fact]
    public void ToInt32_Value_Converts() => Assert.Equal(5, Reader("F", 5).ToInt32(0));

    [Fact]
    public void AsInt64_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsInt64(0));

    [Fact]
    public void AsInt64_Value_ReturnsValue() => Assert.Equal(5L, Reader("F", 5L).AsInt64(0));

    [Fact]
    public void ToInt64_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToInt64(0));

    [Fact]
    public void ToInt64_Value_Converts() => Assert.Equal(5L, Reader("F", 5).ToInt64(0));

    [Fact]
    public void AsString_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).AsString(0));

    [Fact]
    public void AsString_Value_ReturnsValue() => Assert.Equal("v", Reader("F", "v").AsString(0));

    [Fact]
    public void ToString_DbNull_ReturnsNull() => Assert.Null(Reader("F", DBNull.Value).ToString(0));

    [Fact]
    public void ToString_Value_Converts() => Assert.Equal("v", Reader("F", "v").ToString(0));

    // Async extensions

    [Fact]
    public async Task ReadAsync_ForDbReader_DelegatesToNative()
    {
        var reader = new MockDbDataReader([new { A = 1 }]);

        Assert.True(await ((IDataReader)reader).ReadAsync(TestContext.Current.CancellationToken));
        Assert.False(await ((IDataReader)reader).ReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NextResultAsync_ForDbReader_DelegatesToNative()
    {
        var reader = new MockDbDataReader([new { A = 1 }]);

        Assert.False(await ((IDataReader)reader).NextResultAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReadAsync_NullReader_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            ((IDataReader)null).ReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task NextResultAsync_NullReader_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            ((IDataReader)null).NextResultAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReadAsync_ForPlainIDataReader_FallsBackToSync()
    {
        var reader = new PlainReader();

        Assert.True(await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, reader.ReadCount);
    }

    [Fact]
    public async Task NextResultAsync_ForPlainIDataReader_FallsBackToSync()
    {
        var reader = new PlainReader();

        Assert.True(await reader.NextResultAsync(TestContext.Current.CancellationToken));
        Assert.True(reader.Next);
    }

    private class PlainReader : IDataReader
    {
        public int ReadCount;
        public bool Next;
        public int FieldCount => 0;
        public object this[int i] => null;
#pragma warning disable
        public object this[string name] => throw new NotImplementedException();
        public int Depth => 0;
        public bool IsClosed => false;
        public int RecordsAffected => 0;
        public void Close() { }
        public bool GetBoolean(int i) => throw new NotImplementedException();
        public byte GetByte(int i) => throw new NotImplementedException();
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public char GetChar(int i) => throw new NotImplementedException();
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) => throw new NotImplementedException();
        public IDataReader GetData(int i) => throw new NotImplementedException();
        public string GetDataTypeName(int i) => throw new NotImplementedException();
        public DateTime GetDateTime(int i) => throw new NotImplementedException();
        public decimal GetDecimal(int i) => throw new NotImplementedException();
        public double GetDouble(int i) => throw new NotImplementedException();
        public System.Collections.IEnumerator GetEnumerator() => throw new NotImplementedException();
        public Type GetFieldType(int i) => throw new NotImplementedException();
        public float GetFloat(int i) => throw new NotImplementedException();
        public Guid GetGuid(int i) => throw new NotImplementedException();
        public short GetInt16(int i) => throw new NotImplementedException();
        public int GetInt32(int i) => throw new NotImplementedException();
        public long GetInt64(int i) => throw new NotImplementedException();
        public string GetName(int i) => throw new NotImplementedException();
        public int GetOrdinal(string name) => throw new NotImplementedException();
        public DataTable GetSchemaTable() => throw new NotImplementedException();
        public string GetString(int i) => throw new NotImplementedException();
        public object GetValue(int i) => throw new NotImplementedException();
        public int GetValues(object[] values) => throw new NotImplementedException();
        public bool IsDBNull(int i) => true;
        public bool NextResult() { Next = true; return true; }
        public bool Read() { ReadCount++; return true; }
        public void Dispose() { }
    }
}
