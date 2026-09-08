using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serenity.JsonConverters;

public class NullAsDefaultJsonConverterTests
{
    private class TestDto
    {
        [JsonConverter(typeof(NullAsDefaultJsonConverter))]
        public int Value { get; set; }
    }

    [Fact]
    public void CanConvert_ReturnsTrue_ForValueType()
    {
        Assert.True(NullAsDefaultJsonConverter.Instance.CanConvert(typeof(int)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForReferenceType()
    {
        Assert.False(NullAsDefaultJsonConverter.Instance.CanConvert(typeof(string)));
    }

    [Fact]
    public void Deserializes_Null_ToDefault()
    {
        var dto = JsonSerializer.Deserialize<TestDto>("{\"Value\":null}");
        Assert.Equal(0, dto.Value);
    }

    [Fact]
    public void Deserializes_Value_ToValue()
    {
        var dto = JsonSerializer.Deserialize<TestDto>("{\"Value\":42}");
        Assert.Equal(42, dto.Value);
    }

    [Fact]
    public void Serializes_Value_ToNumber()
    {
        var json = JsonSerializer.Serialize(new TestDto { Value = 42 });
        Assert.Contains("42", json);
    }
}