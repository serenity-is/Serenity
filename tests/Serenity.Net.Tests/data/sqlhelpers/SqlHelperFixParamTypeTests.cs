namespace Serenity.Data;

public class SqlHelperFixParamTypeTests
{
    [Fact]
    public void FixParamType_Null_ReturnsDBNull()
    {
        Assert.Equal(DBNull.Value, SqlHelper.FixParamType(null));
    }

    [Fact]
    public void FixParamType_MemoryStream_ReturnsByteArray()
    {
        using var stream = new System.IO.MemoryStream([1, 2, 3]);

        var result = SqlHelper.FixParamType(stream);

        var bytes = Assert.IsType<byte[]>(result);
        Assert.Equal([1, 2, 3], bytes);
    }

    [Fact]
    public void FixParamType_OtherStream_IsCopiedToByteArray()
    {
        using var stream = new CustomStream([4, 5, 6]);

        var result = SqlHelper.FixParamType(stream);

        var bytes = Assert.IsType<byte[]>(result);
        Assert.Equal([4, 5, 6], bytes);
    }

    [Fact]
    public void FixParamType_EmptyMemoryStream_ReturnsEmptyArray()
    {
        using var stream = new System.IO.MemoryStream();

        var result = SqlHelper.FixParamType(stream);

        var bytes = Assert.IsType<byte[]>(result);
        Assert.Empty(bytes);
    }

    [Fact]
    public void FixParamType_IntBackedEnum_ReturnsInt()
    {
        var result = SqlHelper.FixParamType(SampleEnum.Second);

        Assert.Equal(2, result);
    }

    [Fact]
    public void FixParamType_ShortBackedEnum_ReturnsShort()
    {
        var result = SqlHelper.FixParamType(ShortEnum.Value);

        Assert.Equal((short)5, result);
    }

    [Fact]
    public void FixParamType_LongBackedEnum_ReturnsLong()
    {
        var result = SqlHelper.FixParamType(LongEnum.Value);

        Assert.Equal(7L, result);
    }

    [Fact]
    public void FixParamType_ByteBackedEnum_ReturnsByte()
    {
        var result = SqlHelper.FixParamType(ByteEnum.Value);

        Assert.Equal((byte)3, result);
    }

    [Fact]
    public void FixParamType_PlainValues_PassThrough()
    {
        Assert.Equal(5, SqlHelper.FixParamType(5));
        Assert.Equal("test", SqlHelper.FixParamType("test"));
        Assert.Equal(1.5, SqlHelper.FixParamType(1.5));
        Assert.Equal(true, SqlHelper.FixParamType(true));
        Assert.Equal(new DateTime(2024, 1, 15), SqlHelper.FixParamType(new DateTime(2024, 1, 15)));
        Assert.Equal(new Guid("0b69cd5b-1234-5678-9abc-def012345678"),
            SqlHelper.FixParamType(new Guid("0b69cd5b-1234-5678-9abc-def012345678")));
    }

    private class CustomStream(byte[] data) : System.IO.Stream
    {
        private long position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => data.Length;
        public override long Position { get => position; set => position = value; }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var remaining = data.Length - position;
            var toRead = Math.Min(count, remaining);
            Array.Copy(data, position, buffer, offset, toRead);
            position += toRead;
            return (int)toRead;
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }

    private enum ShortEnum : short
    {
        Value = 5
    }

    private enum LongEnum : long
    {
        Value = 7
    }

    private enum ByteEnum : byte
    {
        Value = 3
    }
}
