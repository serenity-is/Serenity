using System.Text.Json;
using Newtonsoft.Json;

namespace Serenity.Data;

public class JsonFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.AJson;
        Assert.Equal("AJson", field.Name);
        Assert.Equal(FieldType.Object, field.Type);
        Assert.Equal("T0.[AJson]", field.Expression);
        Assert.Equal(12, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = JsonField<AllFieldsRow.SampleJson>.Factory(fields, "Test", null, 0, FieldFlags.Default, null!, null!);
        Assert.Equal("Test", field.Name);
        Assert.Null(field.SerializerOptions);
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        using var reader = new MockDbDataReader([new { AJson = (string?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_JsonString_Deserializes()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        using var reader = new MockDbDataReader(new { AJson = "{\"name\":\"x\",\"value\":5}" });
        reader.Read();
        field.GetFromReader(reader, 0, row);
        var value = Assert.IsType<AllFieldsRow.SampleJson>(field[row]);
        Assert.Equal("x", value.Name);
        Assert.Equal(5, value.Value);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AJson.GetFromReader(null!, 0, row));
    }

    [Fact]
    public void AsSqlValue_Null_ReturnsNull()
    {
        var row = NewRow();
        Assert.Null(AllFieldsRow.Fields.AJson.AsSqlValue(row));
    }

    [Fact]
    public void AsSqlValue_Value_SerializesToJson()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson { Name = "y", Value = 3 };
        Assert.Equal("{\"name\":\"y\",\"value\":3}", field.AsSqlValue(row));
    }

    [Fact]
    public void AsSqlValue_CustomOptions_Used()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field.SerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        field[row] = new AllFieldsRow.SampleJson { Name = "y", Value = 3 };
        Assert.Equal("{\"name\":\"y\",\"value\":3}", field.AsSqlValue(row));
    }

    [Fact]
    public void IndexCompare_BothNull_ReturnsZero()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        Assert.Equal(0, AllFieldsRow.Fields.AJson.IndexCompare(row1, row2));
    }

    [Fact]
    public void IndexCompare_NullValues_SortFirst()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        AllFieldsRow.Fields.AJson[row2] = new AllFieldsRow.SampleJson { Name = "x" };
        Assert.Equal(-1, AllFieldsRow.Fields.AJson.IndexCompare(row1, row2));
        Assert.Equal(1, AllFieldsRow.Fields.AJson.IndexCompare(row2, row1));
    }

    [Fact]
    public void IndexCompare_Values_CompareByStringifiedJson()
    {
        var row1 = NewRow();
        var row2 = NewRow();
        AllFieldsRow.Fields.AJson[row1] = new AllFieldsRow.SampleJson { Name = "a", Value = 1 };
        AllFieldsRow.Fields.AJson[row2] = new AllFieldsRow.SampleJson { Name = "b", Value = 1 };
        Assert.True(AllFieldsRow.Fields.AJson.IndexCompare(row1, row2) < 0);
        Assert.True(AllFieldsRow.Fields.AJson.IndexCompare(row2, row1) > 0);
        Assert.Equal(0, AllFieldsRow.Fields.AJson.IndexCompare(row1, row1));
    }

    [Fact]
    public void NewtonsoftValueToJson_WithValue_WritesJson()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson { Name = "z", Value = 9 };
        var sw = new System.IO.StringWriter();
        var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, new Newtonsoft.Json.JsonSerializer());
        writer.Flush();
        Assert.Contains("\"Name\":\"z\"", sw.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void NewtonsoftValueToJson_WithNull_WritesNull()
    {
        var row = NewRow();
        var sw = new System.IO.StringWriter();
        var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        AllFieldsRow.Fields.AJson.ValueToJson(writer, row, new Newtonsoft.Json.JsonSerializer());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Fact]
    public void NewtonsoftValueFromJson_NullToken_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson();
        var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();

        field.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());

        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void NewtonsoftValueFromJson_UndefinedToken_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson();
        var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("undefined"));
        reader.Read();

        field.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());

        Assert.Null(field[row]);
    }

    [Fact]
    public void NewtonsoftValueFromJson_StringToken_ParsesEmbeddedJson()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"{\\\"Name\\\":\\\"s\\\",\\\"Value\\\":7}\""));
        reader.Read();

        field.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());

        var value = Assert.IsType<AllFieldsRow.SampleJson>(field[row]);
        Assert.Equal("s", value.Name);
        Assert.Equal(7, value.Value);
    }

    [Fact]
    public void NewtonsoftValueFromJson_ObjectToken_DeserializesDirectly()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"Name\":\"o\",\"Value\":8}"));
        reader.Read();

        field.ValueFromJson(reader, row, new Newtonsoft.Json.JsonSerializer());

        var value = Assert.IsType<AllFieldsRow.SampleJson>(field[row]);
        Assert.Equal("o", value.Name);
        Assert.Equal(8, value.Value);
    }

    [Fact]
    public void Utf8ValueToJson_Null_WritesNull()
    {
        var row = NewRow();
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        AllFieldsRow.Fields.AJson.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Utf8ValueToJson_Value_WritesJson()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson { Name = "u", Value = 2 };
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        Assert.Equal("{\"Name\":\"u\",\"Value\":2}", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Utf8ValueFromJson_NullToken_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        field[row] = new AllFieldsRow.SampleJson();
        var reader = ReadFirst("null");
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void Utf8ValueFromJson_ObjectToken_Deserializes()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        var reader = ReadFirst("{\"Name\":\"uh\"}");
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        var value = Assert.IsType<AllFieldsRow.SampleJson>(field[row]);
        Assert.Equal("uh", value.Name);
    }

    [Fact]
    public void Utf8ValueFromJson_StringToken_ForNonStringType_ParsesEmbeddedJson()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AJson;
        var reader = ReadFirst("\"{}\"");
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.NotNull(field[row]);
    }

    [Fact]
    public void Utf8ValueFromJson_StringToken_ForStringValueType()
    {
        string? stored = null;
        var field = new JsonField<string>(null!, "TestString", getValue: _ => stored, setValue: (_, v) => stored = v);
        var row = NewRow();
        var reader = ReadFirst("\"raw string\"");
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal("raw string", field[row]);
    }

    private static Utf8JsonReader ReadFirst(string json)
    {
        var raw = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(raw);
        reader.Read();
        return reader;
    }
}
