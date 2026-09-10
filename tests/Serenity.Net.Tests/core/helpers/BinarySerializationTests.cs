namespace Serenity.Data;

public class BinarySerializationTests
{
    [Fact]
    public void Serialize_WritesDataToByteArray()
    {
        var data = BinarySerialization.Serialize(writer =>
        {
            writer.Write(42);
            writer.Write("hello");
        });

        Assert.NotEmpty(data);
    }

    [Fact]
    public void Deserialize_ReadsDataFromByteArray()
    {
        var data = BinarySerialization.Serialize(writer =>
        {
            writer.Write(42);
            writer.Write("hello");
        });

        var result = BinarySerialization.Deserialize(data, reader =>
        {
            var number = reader.ReadInt32();
            var text = reader.ReadString();
            return (number, text);
        });

        Assert.Equal(42, result.number);
        Assert.Equal("hello", result.text);
    }

    [Fact]
    public void Serialize_ThenDeserialize_RoundTrips()
    {
        var data = BinarySerialization.Serialize(writer =>
        {
            writer.Write(123456789L);
            writer.Write(3.14);
        });

        var (longValue, doubleValue) = BinarySerialization.Deserialize(data, reader =>
            (reader.ReadInt64(), reader.ReadDouble()));

        Assert.Equal(123456789L, longValue);
        Assert.Equal(3.14, doubleValue);
    }
}
