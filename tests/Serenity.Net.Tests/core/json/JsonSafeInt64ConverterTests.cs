using Newtonsoft.Json;

namespace Serenity.Data;

public class JsonSafeInt64ConverterTests
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Converters = { new JsonSafeInt64Converter() }
    };

    [Fact]
    public void CanConvert_ReturnsTrue_ForLong()
    {
        Assert.True(new JsonSafeInt64Converter().CanConvert(typeof(long)));
    }

    [Fact]
    public void CanConvert_ReturnsTrue_ForNullableLong()
    {
        Assert.True(new JsonSafeInt64Converter().CanConvert(typeof(long?)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForOtherType()
    {
        Assert.False(new JsonSafeInt64Converter().CanConvert(typeof(int)));
    }

    [Fact]
    public void CanRead_ReturnsFalse()
    {
        Assert.False(new JsonSafeInt64Converter().CanRead);
    }

    [Fact]
    public void CanWrite_ReturnsTrue()
    {
        Assert.True(new JsonSafeInt64Converter().CanWrite);
    }

    [Fact]
    public void Serializes_Null_ToNull()
    {
        Assert.Equal("null", JsonConvert.SerializeObject((object)null, settings));
    }

    [Fact]
    public void Serializes_SmallValue_ToNumber()
    {
        Assert.Equal("42", JsonConvert.SerializeObject(42L, settings));
    }

    [Fact]
    public void Serializes_LargeValue_ToString()
    {
        Assert.Equal("\"9007199254740993\"", JsonConvert.SerializeObject(9007199254740993L, settings));
        Assert.Equal("\"-9007199254740993\"", JsonConvert.SerializeObject(-9007199254740993L, settings));
    }
}