using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class Int64FieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new Int64Field(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Int64, field.Type);
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
        var field = new Int64Field(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = Int64Field.Factory(fields, "Test", null, 0, FieldFlags.Default, null!, null!);
        Assert.IsType<Int64Field>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field[row] = 5L;
        Assert.Equal(5L, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        using var reader = new MockDbDataReader([new { AInt64 = (long?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        using var reader = new MockDbDataReader([new { AInt64 = 5L }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(5L, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AInt64.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(5L, null, 1)]
    [InlineData(null, 5L, -1)]
    [InlineData(5L, 5L, 0)]
    [InlineData(4L, 5L, -1)]
    [InlineData(5L, 4L, 1)]
    public void IndexCompare_Works(long? v1, long? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field[row1] = v1;
        field[row2] = v2;
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field[row] = 42L;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("42", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_LargeValue_WritesAsString()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field[row] = 9007199254740993L;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"9007199254740993\"", sw.ToString());
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("42", 42L)]
    [InlineData("12.4", 12L)]
    [InlineData("true", 1L)]
    [InlineData("false", 0L)]
    [InlineData("\"42\"", 42L)]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, long? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AInt64.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("42", 42L)]
    [InlineData("true", 1L)]
    [InlineData("false", 0L)]
    [InlineData("\"42\"", 42L)]
    public void ValueFromJson_Stj_ParsesTokens(string json, long? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
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
        var field = AllFieldsRow.Fields.AInt64;
        field[row] = 42L;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("42", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
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
        var field = AllFieldsRow.Fields.AInt64;
        field[source] = 5L;
        field.Copy(source, target);
        Assert.Equal(5L, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field[source] = 5L;
        field.CopyNoAssignment(source, target);
        Assert.Equal(5L, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        field.AsObject(row, 5L);
        Assert.Equal(5L, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt64;
        Assert.True(field.IsNull(row));
        field[row] = 5L;
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.AInt64;
        Assert.Equal(5L, field.ConvertValue(5L, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ChangeType_Converts()
    {
        var field = AllFieldsRow.Fields.AInt64;
        Assert.Equal(5L, field.ConvertValue("5", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_JValue_Unwraps()
    {
        var field = AllFieldsRow.Fields.AInt64;
        Assert.Equal(5L, field.ConvertValue(new Newtonsoft.Json.Linq.JValue(5), CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EnumType_SetGet_Works()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.AInt64;
        Assert.Null(field.EnumType);
        field.EnumType = typeof(SampleEnum);
        Assert.Equal(typeof(SampleEnum), field.EnumType);
        field.EnumType = null;
        Assert.Null(field.EnumType);
    }
}
