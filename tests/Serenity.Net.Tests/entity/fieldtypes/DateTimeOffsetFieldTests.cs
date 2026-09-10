using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class DateTimeOffsetFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new DateTimeOffsetField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.DateTime, field.Type);
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
        var field = new DateTimeOffsetField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = DateTimeOffsetField.Factory(fields, "Test", null, 0, FieldFlags.Default, null, null);
        Assert.IsType<DateTimeOffsetField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[row] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new MockDbDataReader([new { ADateTimeOffset = (DateTimeOffset?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new MockDbDataReader([new { ADateTimeOffset = (DateTimeOffset?)new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
    }

    [Fact]
    public void GetFromReader_DateTimeValue_ConvertsToDateTimeOffset()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new MockDbDataReader([new { ADateTimeOffset = (DateTime?)new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero), field[row]);
    }

    [Fact]
    public void GetFromReader_StringValue_Converts()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new MockDbDataReader([new { ADateTimeOffset = "2024-01-15T10:30:00+02:00" }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ADateTimeOffset.GetFromReader(null, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData("2024-01-15T10:30:00+02:00", null, 1)]
    [InlineData(null, "2024-01-15T10:30:00+02:00", -1)]
    [InlineData("2024-01-15T10:30:00+02:00", "2024-01-15T10:30:00+02:00", 0)]
    [InlineData("2024-01-15T09:30:00+02:00", "2024-01-15T10:30:00+02:00", -1)]
    [InlineData("2024-01-15T10:30:00+02:00", "2024-01-15T09:30:00+02:00", 1)]
    public void IndexCompare_Works(string? v1, string? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[row1] = v1 == null ? null : DateTimeOffset.Parse(v1, CultureInfo.InvariantCulture, DateTimeStyles.None);
        field[row2] = v2 == null ? null : DateTimeOffset.Parse(v2, CultureInfo.InvariantCulture, DateTimeStyles.None);
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[row] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.0000000+02:00\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
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
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00+02:00\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_WhitespaceString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"   \""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Date_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00Z\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Date_DateTimeOffsetValue_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00+02:00\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ADateTimeOffset.ValueFromJson(null, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Stj_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var bytes = Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var bytes = Encoding.UTF8.GetBytes("\"2024-01-15T10:30:00Z\"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero), field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_WhitespaceString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var bytes = Encoding.UTF8.GetBytes("\"   \"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var bytes = Encoding.UTF8.GetBytes("{\"a\":1}");
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
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[row] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.0000000\\u002B02:00\"", Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Copy_CopiesValue()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[source] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        field.Copy(source, target);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field[source] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        field.CopyNoAssignment(source, target);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        field.AsObject(row, new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)));
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)), field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        Assert.True(field.IsNull(row));
        field[row] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_Null_ReturnsNull()
    {
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_DateTime_PassesThrough()
    {
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        Assert.Equal(dt, field.ConvertValue(dt, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_DateTimeOffset_ConvertsToDateTime()
    {
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field.ConvertValue(dto, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_String_WithOffset_Parses()
    {
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)),
            field.ConvertValue("2024-01-15T10:30:00+02:00", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_JValue_Unwraps()
    {
        var field = AllFieldsRow.Fields.ADateTimeOffset;
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0),
            field.ConvertValue(new Newtonsoft.Json.Linq.JValue(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2))), CultureInfo.InvariantCulture));
    }
}
