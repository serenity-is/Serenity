using System.Text.Json;

namespace Serenity.JsonConverters;

[Collection("RowJsonConverterStaticHooks")]
public class RowJsonConverterTestsMore
{
    private static JsonSerializerOptions NewOptions(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions();
        options.Converters.Add(new RowJsonConverter());
        return options;
    }

    [Fact]
    public void Write_NullRow_WritesNullValue()
    {
        using var stream = new System.IO.MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new RowJsonConverter().Write(writer, null!, NewOptions());
            writer.Flush();
        }

        Assert.Equal("null", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [Fact]
    public void Read_NullToken_Directly_ReturnsNull()
    {
        var reader = new Utf8JsonReader("null"u8);
        reader.Read();

        Assert.Null(new RowJsonConverter().Read(ref reader, typeof(IdNameRow), NewOptions()));
    }

    [Fact]
    public void ShouldDeserializeExtension_Handles_All_Token_Types()
    {
        var old = RowJsonConverter.SetLocalShouldDeserializeExtension((_, _) => true);
        try
        {
            var row = JsonSerializer.Deserialize<IdNameRow>(
                """{"E1":null,"E2":true,"E7":false,"E3":1.5,"E4":"s","E5":[1,2],"E6":{"a":1}}""", NewOptions());

            IRow r = row!;
            Assert.Null(r.GetDictionaryData("E1"));
            Assert.True((bool)r.GetDictionaryData("E2")!);
            Assert.False((bool)r.GetDictionaryData("E7")!);
            Assert.Equal(1.5, (double)r.GetDictionaryData("E3")!);
            Assert.Equal("s", (string?)r.GetDictionaryData("E4"));
            Assert.Equal(2, Assert.IsType<object[]>(r.GetDictionaryData("E5")).Length);
            Assert.True(Assert.IsType<Dictionary<string, object?>>(r.GetDictionaryData("E6")).ContainsKey("a"));
        }
        finally
        {
            RowJsonConverter.SetLocalShouldDeserializeExtension(old);
        }
    }
}
