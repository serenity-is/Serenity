using System.Text.Json;

namespace Serenity.JsonConverters;

public class ObjectJsonConverterTests
{
    private static JsonSerializerOptions Options()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(ObjectJsonConverter.Instance);
        return options;
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Boolean_Deserializes_ToBool(string json, bool expected)
    {
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<bool>(result);
        Assert.Equal(expected, (bool)result);
    }

    [Theory]
    [InlineData("0", 0L)]
    [InlineData("-1", -1L)]
    [InlineData("9007199254740991", 9007199254740991L)]
    public void Integer_Deserializes_ToLong(string json, long expected)
    {
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<long>(result);
        Assert.Equal(expected, (long)result);
    }

    [Theory]
    [InlineData("1.5", 1.5)]
    [InlineData("-1234567.8901", -1234567.8901)]
    [InlineData("1e20", 1e20)]
    public void NumberWithFractionOrScientific_Deserializes_ToDouble(string json, double expected)
    {
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<double>(result);
        Assert.Equal(expected, (double)result, 6);
    }

    [Fact]
    public void String_Deserializes_ToString()
    {
        var result = JsonSerializer.Deserialize<object>(""" "hello world" """, Options());
        Assert.Equal("hello world", (string)result);
    }

    [Fact]
    public void Null_Deserializes_ToNull()
    {
        var result = JsonSerializer.Deserialize<object>("null", Options());
        Assert.Null(result);
    }

    [Fact]
    public void Array_Deserializes_ToObjectArray_WithElementTypesPreserved()
    {
        const string json = """[1, "x", true, 1.5, "2020-01-02T03:04:05Z"]""";
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<object[]>(result);
        var arr = (object[])result;
        Assert.Equal(5, arr.Length);
        Assert.IsType<long>(arr[0]);
        Assert.Equal(1L, (long)arr[0]);
        Assert.IsType<string>(arr[1]);
        Assert.Equal("x", (string)arr[1]);
        Assert.IsType<bool>(arr[2]);
        Assert.True((bool)arr[2]);
        Assert.IsType<double>(arr[3]);
        Assert.Equal(1.5, (double)arr[3], 6);
        Assert.IsType<DateTimeOffset>(arr[4]);
        Assert.Equal(DateTimeOffset.Parse("2020-01-02T03:04:05Z"), (DateTimeOffset)arr[4]);
    }

    [Fact]
    public void Object_Deserializes_ToDictionary_WithNestedValues()
    {
        const string json = """{"n":123,"s":"hello","b":false,"dt":"2020-01-02T03:04:05+02:00","arr":[1,2],"obj":{"x":1}}""";
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(json, Options());
        Assert.NotNull(result);
        Assert.IsType<long>(result["n"]);
        Assert.Equal(123L, (long)result["n"]);
        Assert.IsType<string>(result["s"]);
        Assert.Equal("hello", (string)result["s"]);
        Assert.IsType<bool>(result["b"]);
        Assert.False((bool)result["b"]);
        Assert.IsType<DateTimeOffset>(result["dt"]);
        Assert.Equal(DateTimeOffset.Parse("2020-01-02T03:04:05+02:00"), (DateTimeOffset)result["dt"]);
        Assert.IsType<object[]>(result["arr"]);
        var arr = (object[])result["arr"];
        Assert.Equal(2, arr.Length);
        Assert.IsType<long>(arr[0]);
        Assert.Equal(1L, (long)arr[0]);
        Assert.IsType<Dictionary<string, object>>(result["obj"]);
        var nested = (Dictionary<string, object>)result["obj"];
        Assert.IsType<long>(nested["x"]);
        Assert.Equal(1L, (long)nested["x"]);
    }

    [Theory]
    [InlineData("2020-01-02T03:04:05Z")]
    [InlineData("2020-01-02T03:04:05+02:00")]
    [InlineData("2020-01-02T03:04:05.123Z")]
    [InlineData("1999-12-31T23:59:59Z")]
    [InlineData("2020-01-02T03:04:05.1234567891011121314Z")]
    [InlineData("2020-01-02T03:04:05")]
    public void IsoDateTimeStrings_AreParsed_AsDateTimeOffset(string iso)
    {
        var json = JsonSerializer.Serialize(iso);
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<DateTimeOffset>(result);
        var dto = (DateTimeOffset)result;
        Assert.Equal(DateTimeOffset.Parse(iso), dto);
    }

    [Theory]
    [InlineData("2020-01-02")] // no time
    [InlineData("20200102T030405Z")] // no dashes
    [InlineData("2020-01-02 03:04:05Z")] // space instead of T
    [InlineData("")] // empty
    [InlineData("not-a-date")]
    [InlineData("2020-01-02T03:04")] // too short
    [InlineData("2020-01-02T03:04:1")] // too short
    [InlineData("2020-01-02T03:04:05.12345678910111213141Z")] // too long
    [InlineData("2020-01-02X03:04:05")] // invalid separator
    [InlineData("1234567890123456789")] // all digits, no T separator
    [InlineData("1234567890T23456789")] // all digits, with T separator

    public void NonIsoOrInvalidDateStrings_AreLeftAsString(string s)
    {
        var json = JsonSerializer.Serialize(s);
        var result = JsonSerializer.Deserialize<object>(json, Options());
        Assert.IsType<string>(result);
        Assert.Equal(s, (string)result);
    }
}