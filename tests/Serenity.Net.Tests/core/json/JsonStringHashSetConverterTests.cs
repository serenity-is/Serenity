using Newtonsoft.Json;

namespace Serenity.Services;

public class JsonStringHashSetConverterTests
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Converters = { new JsonStringHashSetConverter() }
    };

    [Fact]
    public void CanConvert_ReturnsTrue_ForHashSetOfString()
    {
        Assert.True(new JsonStringHashSetConverter().CanConvert(typeof(HashSet<string>)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForOtherType()
    {
        Assert.False(new JsonStringHashSetConverter().CanConvert(typeof(List<string>)));
    }

    [Fact]
    public void CanRead_ReturnsTrue()
    {
        Assert.True(new JsonStringHashSetConverter().CanRead);
    }

    [Fact]
    public void CanWrite_ReturnsTrue()
    {
        Assert.True(new JsonStringHashSetConverter().CanWrite);
    }

    [Fact]
    public void Serializes_HashSet_ToArray()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "b" };
        var json = JsonConvert.SerializeObject(set, settings);
        Assert.Contains("a", json);
        Assert.Contains("b", json);
    }

    [Fact]
    public void Serializes_Null_ToNull()
    {
        Assert.Equal("null", JsonConvert.SerializeObject(null, settings));
    }

    [Fact]
    public void Deserializes_Array_ToHashSet()
    {
        var set = JsonConvert.DeserializeObject<HashSet<string>>("[\"a\",\"b\"]", settings);
        Assert.Equal(2, set.Count);
        Assert.Contains("a", set);
        Assert.Contains("b", set);
    }

    [Fact]
    public void Deserializes_Null_ToNull()
    {
        Assert.Null(JsonConvert.DeserializeObject<HashSet<string>>("null", settings));
    }

    [Fact]
    public void Deserializes_IsCaseInsensitive()
    {
        var set = JsonConvert.DeserializeObject<HashSet<string>>("[\"A\",\"a\"]", settings);
        Assert.Single(set);
    }

    [Fact]
    public void Deserializes_NonArray_Throws()
    {
        Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<HashSet<string>>("\"a\"", settings));
    }

    [Fact]
    public void Deserializes_UnexpectedToken_Throws()
    {
        Assert.Throws<JsonSerializationException>(() =>
            JsonConvert.DeserializeObject<HashSet<string>>("[1]", settings));
    }
}