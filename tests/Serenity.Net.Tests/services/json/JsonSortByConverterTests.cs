namespace Serenity.Services;

public class JsonSortByConverterTests
{
    private static readonly Newtonsoft.Json.JsonSerializerSettings Settings = new()
    {
        Converters = { new JsonSortByConverter() }
    };

    [Fact]
    public void CanConvert_Returns_True_For_SortBy()
    {
        var converter = new JsonSortByConverter();
        Assert.True(converter.CanConvert(typeof(SortBy)));
    }

    [Fact]
    public void CanConvert_Returns_False_For_Other_Types()
    {
        var converter = new JsonSortByConverter();
        Assert.False(converter.CanConvert(typeof(int)));
    }

    [Fact]
    public void CanRead_And_CanWrite_Are_True()
    {
        var converter = new JsonSortByConverter();
        Assert.True(converter.CanRead);
        Assert.True(converter.CanWrite);
    }

    [Fact]
    public void Write_Serializes_Field()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SortBy("Name"), Settings);
        Assert.Equal("\"Name\"", json);
    }

    [Fact]
    public void Write_Appends_DESC_When_Descending()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SortBy("Name", true), Settings);
        Assert.Equal("\"Name DESC\"", json);
    }

    [Fact]
    public void Write_Null_Field_As_Empty_String()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SortBy(), Settings);
        Assert.Equal("\"\"", json);
    }

    [Fact]
    public void Read_Null_Returns_Null()
    {
        var sortBy = Newtonsoft.Json.JsonConvert.DeserializeObject<SortBy>("null", Settings);
        Assert.Null(sortBy);
    }

    [Fact]
    public void Read_Field_Returns_Ascending()
    {
        var sortBy = Newtonsoft.Json.JsonConvert.DeserializeObject<SortBy>("\"Name\"", Settings);
        Assert.NotNull(sortBy);
        Assert.Equal("Name", sortBy.Field);
        Assert.False(sortBy.Descending);
    }

    [Fact]
    public void Read_Field_With_DESC_Returns_Descending()
    {
        var sortBy = Newtonsoft.Json.JsonConvert.DeserializeObject<SortBy>("\"Name desc\"", Settings);
        Assert.NotNull(sortBy);
        Assert.Equal("Name", sortBy.Field);
        Assert.True(sortBy.Descending);
    }

    [Fact]
    public void Read_NonString_Throws_JsonSerializationException()
    {
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<SortBy>("123", Settings));
    }
}
