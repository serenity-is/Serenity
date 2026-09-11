using System.Text.Json;
using System.Text.Json.Serialization;

namespace Serenity.JsonConverters;

/// <summary>
/// Tests for the System.Text.Json based <see cref="RowJsonConverter"/>.
/// </summary>
/// <remarks>
/// Both row converter test classes mutate the static ShouldSerializeExtension /
/// ShouldDeserializeExtension hooks, which are shared by both converters
/// (the Newtonsoft one delegates to this one), so they are placed in the same
/// xUnit collection to avoid races between parallel test classes.
/// </remarks>
[Collection("RowJsonConverterStaticHooks")]
public class RowJsonConverterTests
{
    private static RowJsonConverter NewConverter() => new();

    private static JsonSerializerOptions NewOptions(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions();
        options.Converters.Add(NewConverter());
        return options;
    }

    [Theory]
    [InlineData(typeof(IdNameRow), true)]
    [InlineData(typeof(AllFieldsRow), true)]
    [InlineData(typeof(IRow), false)]
    [InlineData(typeof(AbstractRow), false)]
    [InlineData(typeof(string), false)]
    public void CanConvert_ReturnsExpectedResult(Type type, bool expected)
    {
        Assert.Equal(expected, NewConverter().CanConvert(type));
    }

    [Fact]
    public void Serialize_TrackAssignments_WritesOnlyAssignedFields()
    {
        var row = new IdNameRow { ID = 1, Name = "Test" };

        Assert.Equal("""{"ID":1,"Name":"Test"}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Serialize_TrackAssignments_UnassignedFields_AreSkipped()
    {
        var row = new IdNameRow { ID = 1 };

        Assert.Equal("""{"ID":1}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Serialize_TrackAssignments_NoAssignedFields_WritesEmptyObject()
    {
        Assert.Equal("{}", JsonSerializer.Serialize(new IdNameRow(), NewOptions()));
    }

    [Fact]
    public void Serialize_TrackAssignmentsFalse_WritesAllFields()
    {
        var row = new IdNameRow { ID = 1, Name = "Test" };
        ((IRow)row).TrackAssignments = false;

        Assert.Equal("""{"ID":1,"Name":"Test"}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Serialize_TrackAssignmentsFalse_WritesNullsByDefault()
    {
        var row = new IdNameRow();
        ((IRow)row).TrackAssignments = false;

        Assert.Equal("""{"ID":null,"Name":null}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Serialize_AssignedNullField_IsWrittenByDefault()
    {
        // DefaultIgnoreCondition is Never by default, so assigned null fields
        // are written as null unless WhenWritingNull is specified.
        var row = new IdNameRow { ID = 1, Name = null };

        Assert.Equal("""{"ID":1,"Name":null}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Serialize_AssignedNullField_WhenWritingNull_IsSkipped()
    {
        var row = new IdNameRow { ID = 1, Name = null };
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        Assert.Equal("""{"ID":1}""", JsonSerializer.Serialize(row, NewOptions(options)));
    }

    [Fact]
    public void Serialize_TrackAssignmentsFalse_WhenWritingNull_WritesEmptyObject()
    {
        var row = new IdNameRow();
        ((IRow)row).TrackAssignments = false;
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        Assert.Equal("{}", JsonSerializer.Serialize(row, NewOptions(options)));
    }

    [Fact]
    public void Serialize_UsesPropertyName_OverFieldName()
    {
        var fields = new PropertyNameRow.RowFields();
        fields.Initialize(annotations: null, dialect: SqlServer2012Dialect.Instance);
        fields.Code.PropertyName = "CustomCode";
        var row = new PropertyNameRow(fields) { Code = "X" };

        Assert.Equal("""{"CustomCode":"X"}""", JsonSerializer.Serialize(row, NewOptions()));
    }

    [Fact]
    public void Deserialize_SetsFields_TrackAssignments_AndAssignedFields()
    {
        var row = JsonSerializer.Deserialize<IdNameRow>("""{"ID":42,"Name":"Test"}""", NewOptions());

        Assert.NotNull(row);
        Assert.Equal(42, row.ID);
        Assert.Equal("Test", row.Name);
        Assert.True(((IRow)row).TrackAssignments);
        Assert.True(row.IsAssigned(row.GetFields().ID));
        Assert.True(row.IsAssigned(row.GetFields().Name));
    }

    [Fact]
    public void Deserialize_NullToken_ReturnsNullRow()
    {
        Assert.Null(JsonSerializer.Deserialize<IdNameRow>("null", NewOptions()));
    }

    [Fact]
    public void Deserialize_UnknownProperty_IsIgnoredByDefault()
    {
        var row = JsonSerializer.Deserialize<IdNameRow>("""{"Unknown":"Value"}""", NewOptions());

        Assert.NotNull(row);
        Assert.False(((IRow)row).IsAnyFieldAssigned);
    }

    [Fact]
    public void Deserialize_UnknownProperty_Disallow_ThrowsJsonException()
    {
        var options = new JsonSerializerOptions
        {
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<IdNameRow>("""{"Unknown":"Value"}""", NewOptions(options)));
    }

    [Fact]
    public void Deserialize_ArrayJson_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<IdNameRow>("[]", NewOptions()));
    }

    [Fact]
    public void ShouldSerializeExtension_True_WritesDictionaryData()
    {
        var row = new IdNameRow { ID = 1 };
        ((IRow)row).SetDictionaryData("Extra", "Value");
        var old = RowJsonConverter.SetLocalShouldSerializeExtension((_, _) => true);
        try
        {
            var json = JsonSerializer.Serialize(row, NewOptions());

            Assert.Contains("\"Extra\":\"Value\"", json);
        }
        finally
        {
            RowJsonConverter.SetLocalShouldSerializeExtension(old);
        }
    }

    [Fact]
    public void ShouldSerializeExtension_False_SkipsDictionaryData()
    {
        var row = new IdNameRow { ID = 1 };
        ((IRow)row).SetDictionaryData("Extra", "Value");
        var old = RowJsonConverter.SetLocalShouldSerializeExtension((_, _) => false);
        try
        {
            Assert.Equal("""{"ID":1}""", JsonSerializer.Serialize(row, NewOptions()));
        }
        finally
        {
            RowJsonConverter.SetLocalShouldSerializeExtension(old);
        }
    }

    [Fact]
    public void ShouldDeserializeExtension_StoresDictionaryData()
    {
        var old = RowJsonConverter.SetLocalShouldDeserializeExtension((_, key) => key == "Extra");
        try
        {
            var row = JsonSerializer.Deserialize<IdNameRow>("""{"ID":1,"Extra":"Value"}""", NewOptions());

            Assert.NotNull(row);
            Assert.Equal(1, row.ID);
            Assert.Equal("Value", (string?)((IRow)row).GetDictionaryData("Extra"));
        }
        finally
        {
            RowJsonConverter.SetLocalShouldDeserializeExtension(old);
        }
    }

    [Fact]
    public void RoundTrip_IdNameRow_PreservesValues()
    {
        var row = new IdNameRow { ID = 42, Name = "Test" };

        var json = JsonSerializer.Serialize(row, NewOptions());
        var back = JsonSerializer.Deserialize<IdNameRow>(json, NewOptions());

        Assert.NotNull(back);
        Assert.Equal(42, back.ID);
        Assert.Equal("Test", back.Name);
    }

    [Fact]
    public void RoundTrip_AllFieldsRow_PreservesValues()
    {
        var row = new AllFieldsRow
        {
            AString = "Hello",
            AInt32 = 123,
            ABoolean = true,
            ADecimal = 12.5m
        };

        var json = JsonSerializer.Serialize(row, NewOptions());
        var back = JsonSerializer.Deserialize<AllFieldsRow>(json, NewOptions());

        Assert.NotNull(back);
        Assert.Equal("Hello", back.AString);
        Assert.Equal(123, back.AInt32);
        Assert.True(back.ABoolean);
        Assert.Equal(12.5m, back.ADecimal);
    }

    private abstract class AbstractRow : Row<AbstractRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
        }
    }

    private class PropertyNameRow(PropertyNameRow.RowFields fields) : Row<PropertyNameRow.RowFields>(fields)
    {
        public string? Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Code;

            public RowFields()
            {
                Code = new(this, "Code");
            }
        }
    }
}
