using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class StringFieldTests
{
    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new StringField(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.String, field.Type);
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
        var field = new StringField(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Factory_CreatesField()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = StringField.Factory(fields, "Test", null, 0, FieldFlags.Default, null, null);
        Assert.IsType<StringField>(field);
        Assert.Equal("Test", field.Name);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        field[row] = "Hello";
        Assert.Equal("Hello", field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var reader = new MockDbDataReader([new { AString = (string?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var reader = new MockDbDataReader([new { AString = "Value" }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal("Value", field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AString.GetFromReader(null!, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData("A", null, 1)]
    [InlineData(null, "A", -1)]
    [InlineData("A", "A", 0)]
    [InlineData("A", "B", -1)]
    [InlineData("B", "A", 1)]
    public void IndexCompare_Works(string? v1, string? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AString;
        field[row1] = v1;
        field[row2] = v2;
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        field[row] = "Test";
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("\"Test\"", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("\"Test\"", "Test")]
    [InlineData("123", "123")]
    [InlineData("12.5", "12.5")]
    [InlineData("true", "True")]
    [InlineData("false", "False")]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, string? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_Date_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15T10:30:00Z\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal("2024-01-15T10:30:00Z", field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_DateOnly_Parses()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
        serializer.DateFormatString = "yyyy-MM-dd";
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"2024-01-15\""));
        reader.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
        reader.Read();
        field.ValueFromJson(reader, row, serializer);
        Assert.Equal("2024-01-15", field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AString.ValueFromJson(null!, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("\"Test\"", "Test")]
    [InlineData("123", "123")]
    [InlineData("12.5", "12.5")]
    [InlineData("true", "True")]
    [InlineData("false", "False")]
    public void ValueFromJson_Stj_ParsesTokens(string json, string? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
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
        var field = AllFieldsRow.Fields.AString;
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
        var field = AllFieldsRow.Fields.AString;
        field[row] = "Test";
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("\"Test\"", System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
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
        var field = AllFieldsRow.Fields.AString;
        field[source] = "Test";
        field.Copy(source, target);
        Assert.Equal("Test", field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AString;
        field[source] = "Test";
        field.CopyNoAssignment(source, target);
        Assert.Equal("Test", field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        field.AsObject(row, "Test");
        Assert.Equal("Test", field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AString;
        Assert.True(field.IsNull(row));
        field[row] = "Test";
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_PassesThrough()
    {
        var field = AllFieldsRow.Fields.AString;
        Assert.Equal("Test", field.ConvertValue("Test", CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void CriteriaOperators_CreateCriteria()
    {
        var field = AllFieldsRow.Fields.AString;
        Assert.Equal("T0.[AString] IS NULL", field.IsNull().ToString());
        Assert.Equal("T0.[AString] IS NOT NULL", field.IsNotNull().ToString());
        Assert.Equal("(T0.[AString] LIKE @p1)", field.Like("%x%").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] NOT LIKE @p1)", field.NotLike("%x%").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] LIKE @p1)", field.StartsWith("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] LIKE @p1)", field.EndsWith("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] LIKE @p1)", field.Contains("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] NOT LIKE @p1)", field.NotContains("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] IN (@p1,@p2))", field.In(1, 2).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] NOT IN (@p1,@p2))", field.NotIn(1, 2).ToString(new SqlQuery()));
    }

    [Fact]
    public void EqualityOperators_Work()
    {
        var field = AllFieldsRow.Fields.AString;
        var field2 = AllFieldsRow.Fields.AString;
        Assert.Equal("(T0.[AString] = @p1)", (field == 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] = @p1)", (field == "x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] = T0.[AString])", (field == field2).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] != @p1)", (field != 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] != T0.[AString])", (field != field2).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] > @p1)", (field > 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] >= @p1)", (field >= 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] < @p1)", (field < 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] <= @p1)", (field <= 5).ToString(new SqlQuery()));
    }

    [Fact]
    public void Equals_GetHashCode_Work()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.AString;
        var field2 = fields.AString;
        Assert.True(field.Equals(field2));
        Assert.False(field.Equals("test"));
        Assert.Equal(field.GetHashCode(), field2.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsExpression()
    {
        var field = AllFieldsRow.Fields.AString;
        Assert.Equal("T0.[AString]", field.ToString());
    }

    [Fact]
    public void GetTitle_ReturnsCaptionOrAutoText()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = fields.AString;
        field.Caption = "Db.AllFields.AString";
        Assert.Equal("Db.AllFields.AString", field.GetTitle(null).ToString());
    }
}
