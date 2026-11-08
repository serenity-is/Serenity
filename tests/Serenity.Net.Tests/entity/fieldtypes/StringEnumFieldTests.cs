using System.Text.Json;
using Serenity.JsonConverters;
using Serenity.Testing;

namespace Serenity.Data;

public class StringEnumFieldTests
{
    private static WidgetRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new WidgetRow.RowFields();
        var field = new StringEnumField<WidgetCategory>(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.String, field.Type);
        Assert.Equal(100, field.Size);
        Assert.Equal(FieldFlags.NotNull, field.Flags);
        Assert.Same(fields, field.Fields);
        Assert.Equal(typeof(WidgetCategory), field.EnumType);
    }

    [Fact]
    public void Constructor_StandaloneField_HasIndexMinusOne()
    {
        var field = new StringEnumField<WidgetCategory>(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Constructor_NonStringEnumType_Throws()
    {
        var fields = new WidgetRow.RowFields();
        Assert.Throws<InvalidProgramException>(() => new StringEnumField<StringEnum>(fields, "Test"));
    }

    [Fact]
    public void EnumType_CanBeOverridden()
    {
        var field = new StringEnumField<WidgetCategory>(null, "Test") { EnumType = typeof(WidgetCategoryExtended) };

        Assert.Equal(typeof(WidgetCategoryExtended), field.EnumType);
    }

    [Fact]
    public void Field_ImplementsIEnumTypeField()
    {
        // this is what lets PropertyInfoSource and BasicPropertyProcessor infer editor, filtering and formatting
        IEnumTypeField field = WidgetRow.Fields.Category;

        Assert.Equal(typeof(WidgetCategory), field.EnumType);
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;

        field[row] = "charts";
        Assert.Equal("charts", field[row]);
        Assert.True(row.IsAssigned(field));

        field[row] = null;
        Assert.Null(field[row]);
        Assert.True(field.IsNull(row));
    }

    [Fact]
    public void GetFromReader_ReadsUnknownValue()
    {
        // values written by other systems are read as they are, like an undefined value of an enum field
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        using var reader = new MockDbDataReader([new { Category = "unknown" }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);

        Assert.Equal("unknown", field[row]);
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("\"charts\"", "charts")]
    [InlineData("\"tables\"", "tables")]
    [InlineData("\"\"", null)]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, string? expected)
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.Equal(expected, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Theory]
    [InlineData("\"missing\"")]
    [InlineData("\"Charts\"")]
    [InlineData("1")]
    [InlineData("true")]
    [InlineData("\" charts\"")]
    public void ValueFromJson_Newtonsoft_InvalidValue_Throws(string json)
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();

        var ex = Assert.Throws<InvalidCastException>(() =>
            field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
        Assert.Contains("WidgetCategory", ex.Message);
        Assert.False(row.IsAssigned(field));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();

        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() =>
            WidgetRow.Fields.Category.ValueFromJson(null, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("\"outlines\"", "outlines")]
    [InlineData("\"\"", null)]
    public void ValueFromJson_SystemTextJson_ParsesTokens(string json, string? expected)
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());

        Assert.Equal(expected, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Theory]
    [InlineData("\"missing\"")]
    [InlineData("1")]
    [InlineData("false")]
    public void ValueFromJson_SystemTextJson_InvalidValue_Throws(string json)
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        var ex = Record.Exception(() =>
        {
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        });

        Assert.IsType<InvalidCastException>(ex);
        Assert.False(row.IsAssigned(field));
    }

    [Fact]
    public void ValueFromJson_SystemTextJson_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Category;
        var bytes = System.Text.Encoding.UTF8.GetBytes("[1]");

        var ex = Record.Exception(() =>
        {
            var reader = new Utf8JsonReader(bytes);
            reader.Read();
            field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void ValueFromJson_NoEnumType_AcceptsAnyValue()
    {
        var row = NewRow();
        var field = WidgetRow.Fields.Unvalidated;
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"anything\""));
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());

        Assert.Equal("anything", field[row]);
    }

    [Fact]
    public void RowJsonConverter_DeserializesValidValue()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RowJsonConverter());
        var row = JsonSerializer.Deserialize<WidgetRow>(
            """{"ID":1,"Name":"Chart","Category":"charts"}""", options);

        Assert.NotNull(row);
        Assert.Equal("charts", row.Category);
        Assert.Equal(WidgetCategory.Charts, row.Category);
    }

    [Fact]
    public void RowJsonConverter_InvalidValue_Throws()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RowJsonConverter());

        // the field throws like an enum field does for a value that is not one of its members
        Assert.Throws<InvalidCastException>(() => JsonSerializer.Deserialize<WidgetRow>(
            """{"ID":1,"Category":"missing"}""", options));
    }

    [Fact]
    public void RowJsonConverter_SerializesValue()
    {
        var row = new WidgetRow { ID = 1, Category = WidgetCategory.Tables };
        var options = new JsonSerializerOptions();
        options.Converters.Add(new RowJsonConverter());

        Assert.Equal("""{"ID":1,"Category":"tables"}""", JsonSerializer.Serialize(row, options));
    }
}
