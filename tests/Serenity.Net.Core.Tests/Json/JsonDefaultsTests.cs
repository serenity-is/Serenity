using System.Text.Json;

namespace Serenity.Tests.Json;

public partial class JsonDefaultsTests
{
    private static JsonSerializerOptions GetDefaults(string defaults)
    {
        return defaults switch
        {
            nameof(JsonDefaults.Strict) => JsonDefaults.Strict,
            nameof(JsonDefaults.StrictWriteNulls) => JsonDefaults.StrictWriteNulls,
            nameof(JsonDefaults.Tolerant) => JsonDefaults.Tolerant,
            nameof(JsonDefaults.TolerantWriteNulls) => JsonDefaults.TolerantWriteNulls,
            _ => throw new NotSupportedException()
        };
    }

    [Theory]
    [InlineData(nameof(JsonDefaults.Strict))]
    [InlineData(nameof(JsonDefaults.StrictWriteNulls))]
    [InlineData(nameof(JsonDefaults.Tolerant))]
    [InlineData(nameof(JsonDefaults.TolerantWriteNulls))]
    public void Tolerant_Serializes_SmallLongValues_ToNumber(string defaults)
    {
        var options = GetDefaults(defaults);
        Assert.Equal("0", JsonSerializer.Serialize(0, options));
        Assert.Equal("-1", JsonSerializer.Serialize(-1, options));
        Assert.Equal("9007199254740991", JsonSerializer.Serialize(9007199254740991, options));
        Assert.Equal("9007199254740992", JsonSerializer.Serialize(9007199254740992, options));
        Assert.Equal("-9007199254740992", JsonSerializer.Serialize(-9007199254740992, options));
        Assert.Equal("-9007199254740991", JsonSerializer.Serialize(-9007199254740991, options));
    }

    [Theory]
    [InlineData(nameof(JsonDefaults.Strict))]
    [InlineData(nameof(JsonDefaults.StrictWriteNulls))]
    [InlineData(nameof(JsonDefaults.Tolerant))]
    [InlineData(nameof(JsonDefaults.TolerantWriteNulls))]
    public void Tolerant_Serializes_LargeLongValues_ToString(string defaults)
    {
        var options = GetDefaults(defaults);
        Assert.Equal("\"9007199254740993\"", JsonSerializer.Serialize(9007199254740993, options));
        Assert.Equal("\"-9007199254740993\"", JsonSerializer.Serialize(-9007199254740993, options));
        Assert.Equal("\"9007199254740999\"", JsonSerializer.Serialize(9007199254740999, options));
        Assert.Equal("\"99107199254740999\"", JsonSerializer.Serialize(99107199254740999, options));
        Assert.Equal("\"-99107199254740999\"", JsonSerializer.Serialize(-99107199254740999, options));
    }

    [Theory]
    [InlineData(nameof(JsonDefaults.Strict))]
    [InlineData(nameof(JsonDefaults.StrictWriteNulls))]
    [InlineData(nameof(JsonDefaults.Tolerant))]
    [InlineData(nameof(JsonDefaults.TolerantWriteNulls))]
    public void Tolerant_Deserializes_SmallLongValues_Properly(string defaults)
    {
        var options = GetDefaults(defaults);

        Assert.Equal(0, JsonSerializer.Deserialize<long>("0", options));
        Assert.Equal(-1, JsonSerializer.Deserialize<long>("-1", options));
        Assert.Equal(9007199254740991, JsonSerializer.Deserialize<long>("9007199254740991", options));
        Assert.Equal(9007199254740992, JsonSerializer.Deserialize<long>("9007199254740992", options));
        Assert.Equal(-9007199254740992, JsonSerializer.Deserialize<long>("-9007199254740992", options));
        Assert.Equal(-9007199254740991, JsonSerializer.Deserialize<long>("-9007199254740991", options));
    }

    [Theory]
    [InlineData(nameof(JsonDefaults.Strict))]
    [InlineData(nameof(JsonDefaults.StrictWriteNulls))]
    [InlineData(nameof(JsonDefaults.Tolerant))]
    [InlineData(nameof(JsonDefaults.TolerantWriteNulls))]
    public void Tolerant_Deserializes_LargeLongValues_Properly(string defaults)
    {
        var options = GetDefaults(defaults);
        Assert.Equal(9007199254740993, JsonSerializer.Deserialize<long>("\"9007199254740993\"", options));
        Assert.Equal(-9007199254740993, JsonSerializer.Deserialize<long>("\"-9007199254740993\"", options));
        Assert.Equal(9007199254740999, JsonSerializer.Deserialize<long>("\"9007199254740999\"", options));
        Assert.Equal(99107199254740999, JsonSerializer.Deserialize<long>("\"99107199254740999\"", options));
        Assert.Equal(-99107199254740999, JsonSerializer.Deserialize<long>("\"-99107199254740999\"", options));
    }
}