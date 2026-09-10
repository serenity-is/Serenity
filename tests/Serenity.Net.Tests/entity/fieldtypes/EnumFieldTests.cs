using System.Globalization;
using System.Text.Json;

namespace Serenity.Data;

public class EnumFieldTests
{
    private enum LongEnum : long
    {
        A = 1
    }

    private static AllFieldsRow NewRow() => new();

    [Fact]
    public void Constructor_SetsBasicProperties()
    {
        var fields = new AllFieldsRow.RowFields();
        var field = new EnumField<SampleEnum>(fields, "Test", "Db.Test", 100, FieldFlags.NotNull);

        Assert.Equal("Test", field.Name);
        Assert.Equal(FieldType.Int32, field.Type);
        Assert.Equal(100, field.Size);
        Assert.Equal(FieldFlags.NotNull, field.Flags);
        Assert.Equal("T0.[Test]", field.Expression);
        Assert.Same(fields, field.Fields);
        Assert.Equal(21, field.Index);
        Assert.Equal("Db.TestUtils.AllFields.Test", field.AutoTextKey);
        Assert.Equal(typeof(SampleEnum), field.EnumType);
    }

    [Fact]
    public void Constructor_StandaloneField_HasIndexMinusOne()
    {
        var field = new EnumField<SampleEnum>(null, "Test");
        Assert.Equal(-1, field.Index);
    }

    [Fact]
    public void Constructor_NonEnumType_Throws()
    {
        var fields = new AllFieldsRow.RowFields();
        Assert.Throws<InvalidProgramException>(() => new EnumField<int>(fields, "Test"));
    }

    [Fact]
    public void Constructor_NonInt32UnderlyingEnum_Throws()
    {
        var fields = new AllFieldsRow.RowFields();
        Assert.Throws<InvalidProgramException>(() => new EnumField<LongEnum>(fields, "Test"));
    }

    [Fact]
    public void Indexer_GetSet_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field[row] = SampleEnum.First;
        Assert.Equal(SampleEnum.First, field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void Indexer_SetNull_GetReturnsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field[row] = SampleEnum.First;
        field[row] = null;
        Assert.Null(field[row]);
        Assert.True(field.IsNull(row));
    }

    [Fact]
    public void GetFromReader_Null_SetsNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new MockDbDataReader([new { AEnum = (int?)null }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Null(field[row]);
        Assert.True(row.IsAssigned(field));
    }

    [Fact]
    public void GetFromReader_Value_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new MockDbDataReader([new { AEnum = 1 }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(SampleEnum.First, field[row]);
    }

    [Fact]
    public void GetFromReader_NonIntValue_Converts()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new MockDbDataReader([new { AEnum = 1L }]);
        reader.Read();
        field.GetFromReader(reader, 0, row);
        Assert.Equal(SampleEnum.First, field[row]);
    }

    [Fact]
    public void GetFromReader_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AEnum.GetFromReader(null, 0, row));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(1, null, 1)]
    [InlineData(null, 1, -1)]
    [InlineData(1, 1, 0)]
    [InlineData(0, 1, -1)]
    [InlineData(1, 0, 1)]
    public void IndexCompare_Works(int? v1, int? v2, int expectedSign)
    {
        var row1 = NewRow();
        var row2 = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field.AsObject(row1, v1);
        field.AsObject(row2, v2);
        var result = field.IndexCompare(row1, row2);
        Assert.Equal(Math.Sign(expectedSign), Math.Sign(result));
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field[row] = SampleEnum.First;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("1", sw.ToString());
    }

    [Fact]
    public void ValueToJson_Newtonsoft_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var sw = new System.IO.StringWriter();
        using var writer = new Newtonsoft.Json.JsonTextWriter(sw);
        field.ValueToJson(writer, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        writer.Flush();
        Assert.Equal("null", sw.ToString());
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("1", SampleEnum.First)]
    [InlineData("\"First\"", SampleEnum.First)]
    [InlineData("\"1\"", SampleEnum.First)]
    public void ValueFromJson_Newtonsoft_ParsesTokens(string json, SampleEnum? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(json));
        reader.Read();
        field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_InvalidEnumName_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("\"Invalid\""));
        reader.Read();
        Assert.Throws<ArgumentException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        using var reader = new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader("{\"a\":1}"));
        reader.Read();
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => field.ValueFromJson(reader, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Fact]
    public void ValueFromJson_Newtonsoft_NullReader_Throws()
    {
        var row = NewRow();
        Assert.Throws<ArgumentNullException>(() => AllFieldsRow.Fields.AEnum.ValueFromJson(null, row, Newtonsoft.Json.JsonSerializer.CreateDefault()));
    }

    [Theory]
    [InlineData("null", null)]
    [InlineData("1", SampleEnum.First)]
    [InlineData("\"First\"", SampleEnum.First)]
    public void ValueFromJson_Stj_ParsesTokens(string json, SampleEnum? expected)
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        reader.Read();
        field.ValueFromJson(ref reader, row, new JsonSerializerOptions());
        Assert.Equal(expected, field[row]);
    }

    [Fact]
    public void ValueFromJson_Stj_UnexpectedToken_Throws()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
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
        var field = AllFieldsRow.Fields.AEnum;
        field[row] = SampleEnum.First;
        using var ms = new System.IO.MemoryStream();
        using var writer = new Utf8JsonWriter(ms);
        field.ValueToJson(writer, row, new JsonSerializerOptions());
        writer.Flush();
        Assert.Equal("1", Encoding.UTF8.GetString(ms.ToArray()));
    }

    [Fact]
    public void ValueToJson_Stj_WritesNull()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
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
        var field = AllFieldsRow.Fields.AEnum;
        field[source] = SampleEnum.First;
        field.Copy(source, target);
        Assert.Equal(SampleEnum.First, field[target]);
    }

    [Fact]
    public void CopyNoAssignment_CopiesWithoutAssignment()
    {
        var source = NewRow();
        var target = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field[source] = SampleEnum.First;
        field.CopyNoAssignment(source, target);
        Assert.Equal(SampleEnum.First, field[target]);
        Assert.False(target.IsAssigned(field));
    }

    [Fact]
    public void AsObject_SetsValue()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        field.AsObject(row, 1);
        Assert.Equal(SampleEnum.First, field[row]);
        field.AsObject(row, null);
        Assert.Null(field[row]);
    }

    [Fact]
    public void IsNull_Works()
    {
        var row = NewRow();
        var field = AllFieldsRow.Fields.AEnum;
        Assert.True(field.IsNull(row));
        field[row] = SampleEnum.First;
        Assert.False(field.IsNull(row));
    }

    [Fact]
    public void ConvertValue_Converts()
    {
        var field = AllFieldsRow.Fields.AEnum;
        Assert.Equal(1, field.ConvertValue("1", CultureInfo.InvariantCulture));
        Assert.Equal(1, field.ConvertValue(1L, CultureInfo.InvariantCulture));
        Assert.Equal(1, field.ConvertValue(1, CultureInfo.InvariantCulture));
        Assert.Null(field.ConvertValue(null, CultureInfo.InvariantCulture));
    }
}
