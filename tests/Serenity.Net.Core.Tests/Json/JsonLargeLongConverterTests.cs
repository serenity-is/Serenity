using Serenity.JsonConverters;
using System.Text.Json;

namespace Serenity.Tests.Json;

public partial class JsonLargeLongConverterTests
{
    private static readonly JsonSerializerOptions longOptions = new()
    {
        Converters =
        {
            new SafeInt64JsonConverter()
        }
    };

    [Fact]
    public void Serializes_SmallValues_ToNumber()
    {
        Assert.Equal("0", JsonSerializer.Serialize(0, longOptions));
        Assert.Equal("-1", JsonSerializer.Serialize(-1, longOptions));
        Assert.Equal("9007199254740991", JsonSerializer.Serialize(9007199254740991, longOptions));
        Assert.Equal("9007199254740992", JsonSerializer.Serialize(9007199254740992, longOptions));
        Assert.Equal("-9007199254740992", JsonSerializer.Serialize(-9007199254740992, longOptions));
        Assert.Equal("-9007199254740991", JsonSerializer.Serialize(-9007199254740991, longOptions));
    }

    [Fact]
    public void Serializes_LargeValues_ToString()
    {
        Assert.Equal("\"9007199254740993\"", JsonSerializer.Serialize(9007199254740993, longOptions));
        Assert.Equal("\"-9007199254740993\"", JsonSerializer.Serialize(-9007199254740993, longOptions));
        Assert.Equal("\"9007199254740999\"", JsonSerializer.Serialize(9007199254740999, longOptions));
        Assert.Equal("\"99107199254740999\"", JsonSerializer.Serialize(99107199254740999, longOptions));
        Assert.Equal("\"-99107199254740999\"", JsonSerializer.Serialize(-99107199254740999, longOptions));
    }

    [Fact]
    public void Deserializes_SmallValues_Properly()
    {
        Assert.Equal(0, JsonSerializer.Deserialize<long>("0", longOptions));
        Assert.Equal(-1, JsonSerializer.Deserialize<long>("-1", longOptions));
        Assert.Equal(9007199254740991, JsonSerializer.Deserialize<long>("9007199254740991", longOptions));
        Assert.Equal(9007199254740992, JsonSerializer.Deserialize<long>("9007199254740992", longOptions));
        Assert.Equal(-9007199254740992, JsonSerializer.Deserialize<long>("-9007199254740992", longOptions));
        Assert.Equal(-9007199254740991, JsonSerializer.Deserialize<long>("-9007199254740991", longOptions));
    }

    [Fact]
    public void Deserializes_LargeValues_Properly()
    {
        Assert.Equal(9007199254740993, JsonSerializer.Deserialize<long>("\"9007199254740993\"", longOptions));
        Assert.Equal(-9007199254740993, JsonSerializer.Deserialize<long>("\"-9007199254740993\"", longOptions));
        Assert.Equal(9007199254740999, JsonSerializer.Deserialize<long>("\"9007199254740999\"", longOptions));
        Assert.Equal(99107199254740999, JsonSerializer.Deserialize<long>("\"99107199254740999\"", longOptions));
        Assert.Equal(-99107199254740999, JsonSerializer.Deserialize<long>("\"-99107199254740999\"", longOptions));
    }
}