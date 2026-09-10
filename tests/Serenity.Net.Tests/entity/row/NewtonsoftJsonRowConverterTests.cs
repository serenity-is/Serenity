
namespace Serenity.Data;

/// <summary>
/// Tests for the Newtonsoft.Json based <see cref="JsonRowConverter"/>.
/// </summary>
/// <remarks>
/// Both row converter test classes mutate the static ShouldSerializeExtension /
/// ShouldDeserializeExtension hooks, which are shared by both converters
/// (this one delegates to <see cref="Serenity.JsonConverters.RowJsonConverter"/>),
/// so they are placed in the same xUnit collection to avoid races between
/// parallel test classes.
/// </remarks>
[Collection("RowJsonConverterStaticHooks")]
public class NewtonsoftJsonRowConverterTests
{
    private static JsonRowConverter NewConverter() => new();

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
    public void CanRead_And_CanWrite_ReturnTrue()
    {
        var converter = NewConverter();

        Assert.True(converter.CanRead);
        Assert.True(converter.CanWrite);
    }

    [Fact]
    public void Serialize_TrackAssignments_WritesOnlyAssignedFields()
    {
        var row = new IdNameRow { ID = 1, Name = "Test" };

        Assert.Equal("""{"ID":1,"Name":"Test"}""", Newtonsoft.Json.JsonConvert.SerializeObject(row));
    }

    [Fact]
    public void Serialize_TrackAssignments_UnassignedFields_AreSkipped()
    {
        var row = new IdNameRow { ID = 1 };

        Assert.Equal("""{"ID":1}""", Newtonsoft.Json.JsonConvert.SerializeObject(row));
    }

    [Fact]
    public void Serialize_TrackAssignments_NoAssignedFields_WritesEmptyObject()
    {
        Assert.Equal("{}", Newtonsoft.Json.JsonConvert.SerializeObject(new IdNameRow()));
    }

    [Fact]
    public void Serialize_TrackAssignmentsFalse_WritesAllFields()
    {
        var row = new IdNameRow { ID = 1, Name = "Test" };
        ((IRow)row).TrackAssignments = false;

        Assert.Equal("""{"ID":1,"Name":"Test"}""", Newtonsoft.Json.JsonConvert.SerializeObject(row));
    }

    [Fact]
    public void Serialize_AssignedNullField_IsWrittenByDefault()
    {
        // JsonConvert default Newtonsoft.Json.NullValueHandling is Include, and the converter
        // writes assigned null fields when Newtonsoft.Json.NullValueHandling == Include.
        var row = new IdNameRow { ID = 1, Name = null };

        Assert.Equal("""{"ID":1,"Name":null}""", Newtonsoft.Json.JsonConvert.SerializeObject(row));
    }

    [Fact]
    public void Serialize_AssignedNullField_Ignore_IsSkipped()
    {
        var row = new IdNameRow { ID = 1, Name = null };
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        };

        Assert.Equal("""{"ID":1}""", Newtonsoft.Json.JsonConvert.SerializeObject(row, settings));
    }

    [Fact]
    public void Serialize_TrackAssignmentsFalse_Ignore_WritesEmptyObject()
    {
        var row = new IdNameRow();
        ((IRow)row).TrackAssignments = false;
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        };

        Assert.Equal("{}", Newtonsoft.Json.JsonConvert.SerializeObject(row, settings));
    }

    [Fact]
    public void Deserialize_SetsFields_TrackAssignments_AndAssignedFields()
    {
        var row = Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("""{"ID":42,"Name":"Test"}""");

        Assert.NotNull(row);
        Assert.Equal(42, row.ID);
        Assert.Equal("Test", row.Name);
        Assert.True(((IRow)row).TrackAssignments);
        Assert.True(row.IsAssigned(row.GetFields().ID));
        Assert.True(row.IsAssigned(row.GetFields().Name));
    }

    [Fact]
    public void Deserialize_Null_ReturnsNullRow()
    {
        Assert.Null(Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("null"));
    }

    [Fact]
    public void Deserialize_UnknownMember_IsIgnoredByDefault()
    {
        var row = Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("""{"Unknown":"Value"}""");

        Assert.NotNull(row);
        Assert.False(((IRow)row).IsAnyFieldAssigned);
    }

    [Fact]
    public void Deserialize_UnknownMember_Error_ThrowsJsonSerializationException()
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error
        };

        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("""{"Unknown":"Value"}""", settings));
    }

    [Fact]
    public void Deserialize_ArrayJson_ThrowsJsonSerializationException()
    {
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("[]"));
    }

    [Fact]
    public void ShouldSerializeExtension_DelegatesToRowJsonConverterHook()
    {
        try
        {
            Serenity.JsonConverters.RowJsonConverter.ShouldSerializeExtension = null;
            Func<IRow, string, bool> hook = (_, _) => true;

            JsonRowConverter.ShouldSerializeExtension = hook;

            Assert.Same(hook, Serenity.JsonConverters.RowJsonConverter.ShouldSerializeExtension);
        }
        finally
        {
            Serenity.JsonConverters.RowJsonConverter.ShouldSerializeExtension = null;
        }
    }

    [Fact]
    public void ShouldDeserializeExtension_DelegatesToRowJsonConverterHook()
    {
        try
        {
            Serenity.JsonConverters.RowJsonConverter.ShouldDeserializeExtension = null;
            Func<IRow, string, bool> hook = (_, _) => true;

            JsonRowConverter.ShouldDeserializeExtension = hook;

            Assert.Same(hook, Serenity.JsonConverters.RowJsonConverter.ShouldDeserializeExtension);
        }
        finally
        {
            Serenity.JsonConverters.RowJsonConverter.ShouldDeserializeExtension = null;
        }
    }

    [Fact]
    public void Serialize_WithShouldSerializeExtension_WritesDictionaryData()
    {
        var row = new IdNameRow { ID = 1 };
        ((IRow)row).SetDictionaryData("Extra", "Value");
        JsonRowConverter.ShouldSerializeExtension = (_, _) => true;
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(row);

            Assert.Contains("\"Extra\":\"Value\"", json);
        }
        finally
        {
            JsonRowConverter.ShouldSerializeExtension = null;
        }
    }

    [Fact]
    public void Serialize_WithShouldSerializeExtension_False_SkipsDictionaryData()
    {
        var row = new IdNameRow { ID = 1 };
        ((IRow)row).SetDictionaryData("Extra", "Value");
        JsonRowConverter.ShouldSerializeExtension = (_, _) => false;
        try
        {
            Assert.Equal("""{"ID":1}""", Newtonsoft.Json.JsonConvert.SerializeObject(row));
        }
        finally
        {
            JsonRowConverter.ShouldSerializeExtension = null;
        }
    }

    [Fact]
    public void Deserialize_WithShouldDeserializeExtension_StoresDictionaryData()
    {
        JsonRowConverter.ShouldDeserializeExtension = (_, key) => key == "Extra";
        try
        {
            var row = Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>("""{"ID":1,"Extra":"Value"}""");

            Assert.NotNull(row);
            Assert.Equal(1, row.ID);
            Assert.Equal("Value", (string?)((IRow)row).GetDictionaryData("Extra"));
        }
        finally
        {
            JsonRowConverter.ShouldDeserializeExtension = null;
        }
    }

    [Fact]
    public void RoundTrip_IdNameRow_PreservesValues()
    {
        var row = new IdNameRow { ID = 42, Name = "Test" };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(row);
        var back = Newtonsoft.Json.JsonConvert.DeserializeObject<IdNameRow>(json);

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

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(row);
        var back = Newtonsoft.Json.JsonConvert.DeserializeObject<AllFieldsRow>(json);

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
}
