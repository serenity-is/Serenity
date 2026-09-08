using System.Text.Json;

namespace Serenity.JsonConverters;

public class EnumJsonConverterTests
{
    private enum TestEnum
    {
        Zero = 0,
        One = 1,
        Two = 2
    }

    private static readonly JsonSerializerOptions options = new()
    {
        Converters = { EnumJsonConverter.Instance }
    };

    [Fact]
    public void CanConvert_ReturnsTrue_ForEnum()
    {
        Assert.True(EnumJsonConverter.Instance.CanConvert(typeof(TestEnum)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForNonEnum()
    {
        Assert.False(EnumJsonConverter.Instance.CanConvert(typeof(int)));
    }

    [Fact]
    public void Serializes_Enum_AsNumber()
    {
        Assert.Equal("1", JsonSerializer.Serialize(TestEnum.One, options));
        Assert.Equal("2", JsonSerializer.Serialize(TestEnum.Two, options));
    }

    [Fact]
    public void Deserializes_Number_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("1", options));
        Assert.Equal(TestEnum.Two, JsonSerializer.Deserialize<TestEnum>("2", options));
    }

    [Fact]
    public void Deserializes_StringNumber_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("\"1\"", options));
    }

    [Fact]
    public void Deserializes_StringName_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("\"One\"", options));
    }

    [Fact]
    public void Deserializes_True_ToOne()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("true", options));
    }

    [Fact]
    public void Deserializes_False_ToZero()
    {
        Assert.Equal(TestEnum.Zero, JsonSerializer.Deserialize<TestEnum>("false", options));
    }

    [Fact]
    public void Deserializes_InvalidNumber_Throws()
    {
        Assert.Throws<InvalidCastException>(() =>
            JsonSerializer.Deserialize<TestEnum>("99", options));
    }

    [Fact]
    public void Deserializes_UnexpectedToken_Throws()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<TestEnum>("{}", options));
    }
}