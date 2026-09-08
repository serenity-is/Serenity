using System.Text.Json;

namespace Serenity.JsonConverters;

public class HashSetStringJsonConverterTests
{
    private static readonly JsonSerializerOptions options = new()
    {
        Converters = { new HashSetStringJsonConverter() }
    };

    [Fact]
    public void Serializes_HashSet_ToArray()
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "b" };
        var json = JsonSerializer.Serialize(set, options);
        Assert.Contains("a", json);
        Assert.Contains("b", json);
    }

    [Fact]
    public void Serializes_Null_ToNull()
    {
        Assert.Equal("null", JsonSerializer.Serialize<HashSet<string>>(null, options));
    }

    [Fact]
    public void Deserializes_Array_ToHashSet()
    {
        var set = JsonSerializer.Deserialize<HashSet<string>>("[\"a\",\"b\"]", options);
        Assert.Equal(2, set.Count);
        Assert.Contains("a", set);
        Assert.Contains("b", set);
    }

    [Fact]
    public void Deserializes_IsCaseInsensitive()
    {
        var set = JsonSerializer.Deserialize<HashSet<string>>("[\"A\",\"a\"]", options);
        Assert.Single(set);
    }

    [Fact]
    public void Deserializes_NonArray_Throws()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<HashSet<string>>("\"a\"", options));
    }

    [Fact]
    public void Deserializes_UnexpectedToken_Throws()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<HashSet<string>>("[1]", options));
    }
}