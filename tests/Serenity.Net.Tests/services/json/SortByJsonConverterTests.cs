using System.Text.Json;

namespace Serenity.JsonConverters;

public class SortByJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new SortByJsonConverter() }
    };

    [Fact]
    public void Write_Serializes_Field()
    {
        var json = JsonSerializer.Serialize(new SortBy("Name"), Options);
        Assert.Equal("\"Name\"", json);
    }

    [Fact]
    public void Write_Appends_DESC_When_Descending()
    {
        var json = JsonSerializer.Serialize(new SortBy("Name", descending: true), Options);
        Assert.Equal("\"Name DESC\"", json);
    }

    [Fact]
    public void Write_Null_Field_As_Empty_String()
    {
        var json = JsonSerializer.Serialize(new SortBy(), Options);
        Assert.Equal("\"\"", json);
    }

    [Fact]
    public void Read_Null_Returns_Null()
    {
        var sortBy = JsonSerializer.Deserialize<SortBy>("null", Options);
        Assert.Null(sortBy);
    }

    [Fact]
    public void Read_Direct_Null_Token_Returns_Null()
    {
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes("null"));
        reader.Read();

        var result = new SortByJsonConverter().Read(ref reader, typeof(SortBy), Options);
        Assert.Null(result);
    }

    [Fact]
    public void Read_Field_Returns_Ascending()
    {
        var sortBy = JsonSerializer.Deserialize<SortBy>("\"Name\"", Options);
        Assert.NotNull(sortBy);
        Assert.Equal("Name", sortBy.Field);
        Assert.False(sortBy.Descending);
    }

    [Fact]
    public void Read_Field_With_DESC_Returns_Descending()
    {
        var sortBy = JsonSerializer.Deserialize<SortBy>("\"Name desc\"", Options);
        Assert.NotNull(sortBy);
        Assert.Equal("Name", sortBy.Field);
        Assert.True(sortBy.Descending);
    }

    [Fact]
    public void Read_Trims_Whitespace()
    {
        var sortBy = JsonSerializer.Deserialize<SortBy>("\"  Name  \"", Options);
        Assert.NotNull(sortBy);
        Assert.Equal("Name", sortBy.Field);
    }

    [Fact]
    public void Read_NonString_Throws_JsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<SortBy>("123", Options));
    }
}
