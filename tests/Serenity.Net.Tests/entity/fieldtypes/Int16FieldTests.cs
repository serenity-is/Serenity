using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class Int16FieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new Int16Field(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Int16, field.Type);
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
        var field = new Int16Field(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = Int16Field.Factory(fields, "Test", null, 0, FieldFlags.Default, null!, null!);
        Assert.IsType<Int16Field>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        field[row] = 5;
        Assert.Equal((short)5, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        using var reader = new MockDbDataReader([new { AInt16 = (short?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        using var reader = new MockDbDataReader([new { AInt16 = 5 }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal((short)5, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AInt16.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData((short)5, null, 1)]
    [InlineData(null, (short)5, -1)]
    [InlineData((short)5, (short)5, 0)]
    [InlineData((short)4, (short)5, -1)]
    [InlineData((short)5, (short)4, 1)]
    public void IndexCompare_Works(short? v1, short? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        field[row1] = v1;
        field[row2] = v2;
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        field[row] = 42;
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
        var field = AllFieldsRow.Fields.AInt16;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("42", (short)42)]
    [InlineData("12.4", (short)12)]
    [InlineData("true", (short)1)]
    [InlineData("false", (short)0)]
    [InlineData("\"42\"", (short)42)]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, short? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AInt16.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("42", (short)42)]
    [InlineData("true", (short)1)]
    [InlineData("false", (short)0)]
    [InlineData("\"42\"", (short)42)]
    public void ValueFromJson_Stj_ParsesTokens(string json, short? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
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
        var field = AllFieldsRow.Fields.AInt16;
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
        var field = AllFieldsRow.Fields.AInt16;
        field[row] = 42;
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
        var field = AllFieldsRow.Fields.AInt16;
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
        var field = AllFieldsRow.Fields.AInt16;
        field[source] = 5;
        field.Copy(source, target);
        Assert.Equal((short)5, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        field[source] = 5;
        field.CopyNoAssignment(source, target);
        Assert.Equal((short)5, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        field.AsObject(row, (short)5);
        Assert.Equal((short)5, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AInt16;
        Assert.True(field.IsNull(row));
        field[row] = 5;
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.AInt16;
        Assert.Equal((short)5, field.ConvertValue((short)5, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ChangeType_Converts()
    {
        var field = AllFieldsRow.Fields.AInt16;
        Assert.Equal((short)5, field.ConvertValue("5", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_JValue_Unwraps()
    {
        var field = AllFieldsRow.Fields.AInt16;
        Assert.Equal((short)5, field.ConvertValue(new Newtonsoft.Json.Linq.JValue(5), CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EnumType_SetGet_Works()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.AInt16;
        Assert.Null(field.EnumType);
        field.EnumType = typeof(SampleEnum);
        Assert.Equal(typeof(SampleEnum), field.EnumType);
        field.EnumType = null;
        Assert.Null(field.EnumType);
    }
}
