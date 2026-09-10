using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class SingleFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new SingleField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Single, field.Type);
        Assert.Equal(100, field.Size);
        Assert.Equal(FieldFlags.NotNull, field.Flags);
        Assert.Equal("T0.[Test]", field.Expression);
        Assert.Same(fields, field.Fields);
        Assert.Equal(21, field.Index);
        Assert.Equal("Db.TestUtils.AllFields.Test", field.AutoTextKey);
    }

    [Fact]
    public void Constructor_StandaloneField_HasIndexMinusOne()
    {
        var field = new SingleField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = SingleField.Factory(fields, "Test", null, 0, FieldFlags.Default, null, null);
        Assert.IsType<SingleField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[row] = 1.5f;
        Assert.Equal(1.5f, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var reader = new MockDbDataReader([new { ASingle = (float?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var reader = new MockDbDataReader([new { ASingle = (float?)1.5f }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(1.5f, field[row]);
    }

    [Fact]
    public void GetFromReader_NonSingleValue_Converts()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var reader = new MockDbDataReader([new { ASingle = 5 }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(5f, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ASingle.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(1.5, null, 1)]
    [InlineData(null, 1.5, -1)]
    [InlineData(1.5, 1.5, 0)]
    [InlineData(1.5, 2.5, -1)]
    [InlineData(2.5, 1.5, 1)]
    public void IndexCompare_Works(double? v1, double? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[row1] = v1.HasValue ? (float?)v1.Value : null;
        field[row2] = v2.HasValue ? (float?)v2.Value : null;
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[row] = 1.5f;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("1.5", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("\"1.5\"", 1.5)]
    [InlineData("123", 123.0)]
    [InlineData("1.5", 1.5)]
    [InlineData("true", 1.0)]
    [InlineData("false", 0.0)]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, double? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(expected.HasValue ? (float?)expected.Value : null, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ASingle.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("1.5", 1.5)]
    [InlineData("123", 123.0)]
    [InlineData("\"1.5\"", 1.5)]
    [InlineData("true", 1.0)]
    [InlineData("false", 0.0)]
    public void ValueFromJson_Stj_ParsesTokens(string json, double? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(expected.HasValue ? (float?)expected.Value : null, field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        var bytes = System.Text.Encoding.UTF8.GetBytes("{\"a\":1}");
        var ex = Record.Exception(() =>
        {
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        });
        Assert.IsType<JsonException>(ex);
    }

    [Fact]
    public void ValueToJson_Stj_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[row] = 1.5f;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("1.5", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Copy_CopiesValue()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[source] = 1.5f;
        field.Copy(source, target);
        Assert.Equal(1.5f, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field[source] = 1.5f;
        field.CopyNoAssignment(source, target);
        Assert.Equal(1.5f, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        field.AsObject(row, 1.5f);
        Assert.Equal(1.5f, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ASingle;
        Assert.True(field.IsNull(row));
        field[row] = 1.5f;
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.ASingle;
        Assert.Equal(1.5f, field.ConvertValue(1.5f, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ConvertsFromString()
    {
        var field = AllFieldsRow.Fields.ASingle;
        Assert.Equal(1.5f, field.ConvertValue("1.5", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ConvertsFromInt()
    {
        var field = AllFieldsRow.Fields.ASingle;
        Assert.Equal(5f, field.ConvertValue(5, CultureInfo.InvariantCulture));
    }
}
