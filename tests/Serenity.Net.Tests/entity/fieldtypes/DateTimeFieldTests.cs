using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class DateTimeFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new DateTimeField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

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
        var field = new DateTimeField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = DateTimeField.Factory(fields, "Test", null, 0, FieldFlags.Default, null!, null!);
        Assert.IsType<DateTimeField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field[row] = new DateTime(2024, 1, 15, 10, 30, 0);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new MockDbDataReader([new { ADateTime = (DateTime?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new MockDbDataReader([new { ADateTime = (DateTime?)new DateTime(2024, 1, 15, 10, 30, 0) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
    }

    [Fact]
    public void GetFromReader_DateTimeOffsetValue_ConvertsToDateTime()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new MockDbDataReader([new { ADateTime = (DateTimeOffset?)new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
    }

    [Fact]
    public void GetFromReader_StringValue_Converts()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new MockDbDataReader([new { ADateTime = (string?)"2024-01-15" }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTime(2024, 1, 15), field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ADateTime.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData("2024-01-15", null, 1)]
    [InlineData(null, "2024-01-15", -1)]
    [InlineData("2024-01-15", "2024-01-15", 0)]
    [InlineData("2024-01-14", "2024-01-15", -1)]
    [InlineData("2024-01-15", "2024-01-14", 1)]
    public void IndexCompare_Works(string? v1, string? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field[row1] = v1 == null ? (DateTime?)null : DateTime.Parse(v1, CultureInfo.InvariantCulture);
        field[row2] = v2 == null ? (DateTime?)null : DateTime.Parse(v2, CultureInfo.InvariantCulture);
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field[row] = new DateTime(2024, 1, 15, 10, 30, 0);
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.000\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_UtcKind_WritesWithZSuffix()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        field.DateTimeKind = DateTimeKind.Utc;
        var row = NewRow();
        field[row] = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.000Z\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_LocalKind_WritesUniversalTime()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        field.DateTimeKind = DateTimeKind.Local;
        var row = NewRow();
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);
        field[row] = dt;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        var expected = "\"" + dt.ToUniversalTime().ToString(DateHelper.ISODateTimeFormatUTC, CultureInfo.InvariantCulture) + "\"";
        Assert.Equal(expected, sw.ToString());
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("null"));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_String_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_WhitespaceString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
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
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00Z\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Date_DateOnly_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15\""));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTime(2024, 1, 15), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Date_DateTimeOffsetValue_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00+02:00\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.ADateTime.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Stj_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        var bytes = System.Text.Encoding.UTF8.GetBytes("null");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Theory]
    [InlineData("\"2024-01-15T10:30:00Z\"", "2024-01-15T10:30:00")]
    [InlineData("\"2024-01-15\"", "2024-01-15T00:00:00")]
    public void ValueFromJson_Stj_String_Parses(string json, string expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(DateTime.Parse(expected, CultureInfo.InvariantCulture), field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_WhitespaceString_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        var bytes = System.Text.Encoding.UTF8.GetBytes("\"   \"");
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Null(field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
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
        var field = AllFieldsRow.Fields.ADateTime;
        field[row] = new DateTime(2024, 1, 15, 10, 30, 0);
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.000\"", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("null", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_UtcKind_WritesWithZSuffix()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        field.DateTimeKind = DateTimeKind.Utc;
        var row = NewRow();
        field[row] = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"2024-01-15T10:30:00.000Z\"", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void Copy_CopiesValue()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field[source] = new DateTime(2024, 1, 15, 10, 30, 0);
        field.Copy(source, target);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field[source] = new DateTime(2024, 1, 15, 10, 30, 0);
        field.CopyNoAssignment(source, target);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        field.AsObject(row, new DateTime(2024, 1, 15, 10, 30, 0));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.ADateTime;
        Assert.True(field.IsNull(row));
        field[row] = new DateTime(2024, 1, 15);
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_Null_ReturnsNull()
    {
        var field = AllFieldsRow.Fields.ADateTime;
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_DateTime_PassesThrough()
    {
        var field = AllFieldsRow.Fields.ADateTime;
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        Assert.Equal(dt, field.ConvertValue(dt, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_DateTimeOffset_ConvertsToDateTime()
    {
        var field = AllFieldsRow.Fields.ADateTime;
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field.ConvertValue(dto, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_String_Parses()
    {
        var field = AllFieldsRow.Fields.ADateTime;
        Assert.Equal(new DateTime(2024, 1, 15), field.ConvertValue("2024-01-15", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ConvertValue_JValue_Unwraps()
    {
        var field = AllFieldsRow.Fields.ADateTime;
        Assert.Equal(new DateTime(2024, 1, 15), field.ConvertValue(new Newtonsoft.Json.Linq.JValue(new DateTime(2024, 1, 15)), CultureInfo.InvariantCulture));
    }

    [Fact]
    public void DateOnly_Property_DefaultTrue_Toggling_Works()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        Assert.True(field.DateOnly);
        field.DateOnly = false;
        Assert.False(field.DateOnly);
        Assert.Equal(DateTimeKind.Unspecified, field.DateTimeKind);
        field.DateOnly = true;
        Assert.True(field.DateOnly);
    }

    [Fact]
    public void DateTimeKind_Property_SetGet_Works()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        Assert.Equal(DateTimeKind.Unspecified, field.DateTimeKind);
        field.DateTimeKind = DateTimeKind.Utc;
        Assert.Equal(DateTimeKind.Utc, field.DateTimeKind);
        Assert.False(field.DateOnly);
        field.DateTimeKind = DateTimeKind.Local;
        Assert.Equal(DateTimeKind.Local, field.DateTimeKind);
        field.DateTimeKind = DateTimeKind.Unspecified;
        Assert.Equal(DateTimeKind.Unspecified, field.DateTimeKind);
        Assert.False(field.DateOnly);
    }

    [Fact]
    public void ToDateTimeKind_Static_NullOrUnspecifiedKind_ReturnsValueUnchanged()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        Assert.Equal(dt, DateTimeField.ToDateTimeKind(dt, null));
        Assert.Equal(dt, DateTimeField.ToDateTimeKind(dt, DateTimeKind.Unspecified));
    }

    [Fact]
    public void ToDateTimeKind_Static_UtcKind_ReturnsUniversalTime()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        Assert.Equal(dt, DateTimeField.ToDateTimeKind(dt, DateTimeKind.Utc));
    }

    [Fact]
    public void ToDateTimeKind_Static_LocalKind_ReturnsLocalTime()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);
        Assert.Equal(dt, DateTimeField.ToDateTimeKind(dt, DateTimeKind.Local));
    }

    [Fact]
    public void ToDateTimeKind_Static_UtcKindValueWithLocalTarget_Converts()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        Assert.Equal(dt.ToLocalTime(), DateTimeField.ToDateTimeKind(dt, DateTimeKind.Local));
    }

    [Fact]
    public void ToDateTimeKind_Static_DateTimeOffset_NullOrUnspecifiedKind_ReturnsDateTime()
    {
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), DateTimeField.ToDateTimeKind(dto, null));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), DateTimeField.ToDateTimeKind(dto, DateTimeKind.Unspecified));
    }

    [Fact]
    public void ToDateTimeKind_Static_DateTimeOffset_UtcKind_ReturnsUtcDateTime()
    {
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTime(2024, 1, 15, 8, 30, 0), DateTimeField.ToDateTimeKind(dto, DateTimeKind.Utc));
    }

    [Fact]
    public void ToDateTimeKind_Static_DateTimeOffset_LocalKind_ReturnsLocalDateTime()
    {
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(dto.LocalDateTime, DateTimeField.ToDateTimeKind(dto, DateTimeKind.Local));
    }

    [Fact]
    public void ToDateTimeKind_Instance_DateTimeOffset_UsesFieldKind()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field.ToDateTimeKind(dto));
        field.DateTimeKind = DateTimeKind.Utc;
        Assert.Equal(new DateTime(2024, 1, 15, 8, 30, 0), field.ToDateTimeKind(dto));
    }

    [Fact]
    public void ToDateTimeKind_Instance_DateTime_UsesFieldKind()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        var dt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        Assert.Equal(dt, field.ToDateTimeKind(dt));
        field.DateTimeKind = DateTimeKind.Utc;
        Assert.Equal(dt, field.ToDateTimeKind(dt));
    }

    [Fact]
    public void GetFromReader_WithDateTimeKindUtc_SetsKind()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        field.DateTimeKind = DateTimeKind.Utc;
        var row = NewRow();
        using var reader = new MockDbDataReader([new { ADateTime = (DateTime?)new DateTime(2024, 1, 15, 10, 30, 0) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
        Assert.Equal(DateTimeKind.Utc, field[row]!.Value.Kind);
    }

    [Fact]
    public void GetFromReader_WithDateTimeKindLocal_SetsKind()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.ADateTime;
        field.DateTimeKind = DateTimeKind.Local;
        var row = NewRow();
        using var reader = new MockDbDataReader([new { ADateTime = (DateTime?)new DateTime(2024, 1, 15, 10, 30, 0) }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), field[row]);
        Assert.Equal(DateTimeKind.Local, field[row]!.Value.Kind);
    }
}
