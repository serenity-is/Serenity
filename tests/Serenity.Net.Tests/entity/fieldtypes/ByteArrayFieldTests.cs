using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class ByteArrayFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new ByteArrayField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Object, field.Type);
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
        var field = new ByteArrayField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = ByteArrayField.Factory(fields, "Test", null, 0, FieldFlags.Default, null!, null!);
        Assert.IsType<ByteArrayField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field[row] = [1, 2];
        Assert.Equal(new byte[] { 1, 2 }, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        using var reader = new MockDbDataReader([new { AByteArray = (byte[]?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        using var reader = new MockDbDataReader(new { AByteArray = (byte[]?)[1, 2] });
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new byte[] { 1, 2 }, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AByteArray.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData("0102", null, 1)]
    [InlineData(null, "0102", -1)]
    [InlineData("0102", "0102", 0)]
    [InlineData("0102", "0103", -1)]
    [InlineData("0103", "0102", 1)]
    [InlineData("01", "0102", -1)]
    [InlineData("0102", "01", 1)]
    public void IndexCompare_Works(string? hex1, string? hex2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field[row1] = hex1 == null ? null : Convert.FromHexString(hex1);
        field[row2] = hex2 == null ? null : Convert.FromHexString(hex2);
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field[row] = [1, 2];
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"AQI=\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
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
        var field = AllFieldsRow.Fields.AByteArray;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Base64String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"AQI=\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new byte[] { 1, 2 }, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AByteArray.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Stj_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        var bytes = System.Text.Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_Base64String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        var bytes = System.Text.Encoding.UTF8.GetBytes("\"AQI=\"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(new byte[] { 1, 2 }, field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_EmptyString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        var bytes = System.Text.Encoding.UTF8.GetBytes("\"\"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
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
        var field = AllFieldsRow.Fields.AByteArray;
        field[row] = [1, 2];
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"AQI=\"", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Copy_CopiesClonedValue()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        var bytes = new byte[] { 1, 2 };
        field[source] = bytes;
        field.Copy(source, target);
        Assert.Equal(bytes, field[target]);
        Assert.NotSame(bytes, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field[source] = [1, 2];
        field.CopyNoAssignment(source, target);
        Assert.Equal(new byte[] { 1, 2 }, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field.AsObject(row, new byte[] { 1, 2 });
        Assert.Equal(new byte[] { 1, 2 }, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        Assert.True(field.IsNull(row));
        field[row] = [1, 2];
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.AByteArray;
        var bytes = new byte[] { 1, 2 };
        Assert.Same(bytes, field.ConvertValue(bytes, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_NonByteArray_Throws()
    {
        var field = AllFieldsRow.Fields.AByteArray;
        Assert.Throws<InvalidCastException>(() => field.ConvertValue("Test", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AsSqlValue_Null_ReturnsSqlBinaryNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        var value = field.AsSqlValue(row);
        Assert.IsType<System.Data.SqlTypes.SqlBinary>(value);
        Assert.True(((System.Data.SqlTypes.SqlBinary)value!).IsNull);
    }

    [Fact]
    public void AsSqlValue_Value_ReturnsByteArray()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AByteArray;
        field[row] = [1, 2];
        Assert.Equal(new byte[] { 1, 2 }, (byte[])field.AsSqlValue(row)!);
    }
}
