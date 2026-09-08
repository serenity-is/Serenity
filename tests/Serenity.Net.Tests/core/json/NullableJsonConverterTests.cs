using System.Text.Json;

namespace Serenity.JsonConverters;

public class NullableJsonConverterTests
{
    private static readonly JsonSerializerOptions options = new()
    {
        Converters = { NullableJsonConverter.Instance }
    };

    [Fact]
    public void CanConvert_ReturnsTrue_ForNullable()
    {
        Assert.True(NullableJsonConverter.Instance.CanConvert(typeof(int?)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForNonNullable()
    {
        Assert.False(NullableJsonConverter.Instance.CanConvert(typeof(int)));
    }

    [Fact]
    public void Serializes_Null_ToNull()
    {
        Assert.Equal("null", JsonSerializer.Serialize<int?>(null, options));
    }

    [Fact]
    public void Serializes_Value_ToNumber()
    {
        Assert.Equal("42", JsonSerializer.Serialize<int?>(42, options));
    }

    [Fact]
    public void Deserializes_Null_ToNull()
    {
        Assert.Null(JsonSerializer.Deserialize<int?>("null", options));
    }

    [Fact]
    public void Deserializes_EmptyString_ToNull()
    {
        Assert.Null(JsonSerializer.Deserialize<int?>("\"\"", options));
    }

    [Fact]
    public void Deserializes_Number_ToValue()
    {
        Assert.Equal(42, JsonSerializer.Deserialize<int?>("42", options));
    }
}