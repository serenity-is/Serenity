using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class GuidFieldTests
{
    private static readonly Guid TestGuid = new("0b69cd5b-1234-5678-9abc-def012345678");

    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new GuidField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Guid, field.Type);
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
        var field = new GuidField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = GuidField.Factory(fields, "Test", null, 0, FieldFlags.Default, null, null);
        Assert.IsType<GuidField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        field[row] = TestGuid;
        Assert.Equal(TestGuid, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new MockDbDataReader([new { AGuid = (Guid?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new MockDbDataReader([new { AGuid = (Guid?)TestGuid }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(TestGuid, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AGuid.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData("00000000-0000-0000-0000-000000000001", null, 1)]
    [InlineData(null, "00000000-0000-0000-0000-000000000001", -1)]
    [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001", 0)]
    [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", -1)]
    [InlineData("00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000001", 1)]
    public void IndexCompare_Works(string? v1, string? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        field[row1] = v1 == null ? null : Guid.Parse(v1);
        field[row2] = v2 == null ? null : Guid.Parse(v2);
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        field[row] = TestGuid;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"" + TestGuid.ToString("D", CultureInfo.InvariantCulture) + "\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"" + TestGuid.ToString("D", CultureInfo.InvariantCulture) + "\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(TestGuid, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_EmptyString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("123"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AGuid.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Stj_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        var bytes = System.Text.Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        var bytes = System.Text.Encoding.UTF8.GetBytes("\"" + TestGuid.ToString("D", CultureInfo.InvariantCulture) + "\"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(TestGuid, field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_EmptyString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        var bytes = System.Text.Encoding.UTF8.GetBytes("\"\"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_NumberToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        var bytes = System.Text.Encoding.UTF8.GetBytes("123");
        var ex = Record.Exception(() =>
        {
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        });
        Assert.IsType<JsonException>(ex);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
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
        var field = AllFieldsRow.Fields.AGuid;
        field[row] = TestGuid;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"" + TestGuid.ToString("D", CultureInfo.InvariantCulture) + "\"", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
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
        var field = AllFieldsRow.Fields.AGuid;
        field[source] = TestGuid;
        field.Copy(source, target);
        Assert.Equal(TestGuid, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        field[source] = TestGuid;
        field.CopyNoAssignment(source, target);
        Assert.Equal(TestGuid, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        field.AsObject(row, TestGuid);
        Assert.Equal(TestGuid, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AGuid;
        Assert.True(field.IsNull(row));
        field[row] = TestGuid;
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.AGuid;
        Assert.Equal(TestGuid, field.ConvertValue(TestGuid, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ConvertsFromString()
    {
        var field = AllFieldsRow.Fields.AGuid;
        Assert.Equal(TestGuid, field.ConvertValue(TestGuid.ToString("D", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue("", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ConvertsFromByteArray()
    {
        var field = AllFieldsRow.Fields.AGuid;
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        Assert.Equal(new Guid(bytes), field.ConvertValue(bytes, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_ConvertsFromJValue()
    {
        var field = AllFieldsRow.Fields.AGuid;
        var jValue = new Newtonsoft.Json.Linq.JValue(TestGuid.ToString("D", CultureInfo.InvariantCulture));
        Assert.Equal(TestGuid, field.ConvertValue(jValue, CultureInfo.InvariantCulture));
    }
}
