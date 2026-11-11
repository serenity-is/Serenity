using System.Text.Json;

namespace Serenity.JsonConverters;

public class EnumJsonConverterTests
{
    private enum TestEnum
    {
        Zero = 0,
        One = 1,
        Two = 2
    }

    [Flags]
    public enum TestFlags
    {
        None = 0,
        Table = 1,
        View = 2,
        Entity = 4
    }

    // a flags enum whose values are not all single bits, like a named combination
    [Flags]
    private enum TestGroupedFlags
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write,
        Admin = 8
    }

    [Flags]
    private enum TestUnsignedFlags : ulong
    {
        None = 0,
        Low = 1,
        High = 1UL << 63
    }

    private static readonly JsonSerializerOptions options = new()
    {
        Converters = { EnumJsonConverter.Instance }
    };

    [Fact]
    public void CanConvert_ReturnsTrue_ForEnum()
    {
        Assert.True(EnumJsonConverter.Instance.CanConvert(typeof(TestEnum)));
    }

    [Fact]
    public void CanConvert_ReturnsFalse_ForNonEnum()
    {
        Assert.False(EnumJsonConverter.Instance.CanConvert(typeof(int)));
    }

    [Fact]
    public void Serializes_Enum_AsNumber()
    {
        Assert.Equal("1", JsonSerializer.Serialize(TestEnum.One, options));
        Assert.Equal("2", JsonSerializer.Serialize(TestEnum.Two, options));
    }

    [Fact]
    public void Deserializes_Number_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("1", options));
        Assert.Equal(TestEnum.Two, JsonSerializer.Deserialize<TestEnum>("2", options));
    }

    [Fact]
    public void Deserializes_StringNumber_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("\"1\"", options));
    }

    [Fact]
    public void Deserializes_StringName_ToEnum()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("\"One\"", options));
    }

    [Fact]
    public void Deserializes_True_ToOne()
    {
        Assert.Equal(TestEnum.One, JsonSerializer.Deserialize<TestEnum>("true", options));
    }

    [Fact]
    public void Deserializes_False_ToZero()
    {
        Assert.Equal(TestEnum.Zero, JsonSerializer.Deserialize<TestEnum>("false", options));
    }

    [Fact]
    public void Deserializes_InvalidNumber_Throws()
    {
        Assert.Throws<InvalidCastException>(() =>
            JsonSerializer.Deserialize<TestEnum>("99", options));
    }

    [Fact]
    public void Deserializes_UnexpectedToken_Throws()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<TestEnum>("{}", options));
    }

    [Theory]
    [InlineData("0", TestFlags.None)]
    [InlineData("1", TestFlags.Table)]
    [InlineData("3", TestFlags.Table | TestFlags.View)]
    [InlineData("5", TestFlags.Table | TestFlags.Entity)]
    [InlineData("7", TestFlags.Table | TestFlags.View | TestFlags.Entity)]
    [InlineData("\"3\"", TestFlags.Table | TestFlags.View)]
    [InlineData("\"Table, View\"", TestFlags.Table | TestFlags.View)]
    public void Deserializes_ACombinationOfTheValuesOfAFlagsEnum(string json, TestFlags expected)
    {
        // the combination is not defined itself, but it is made of the defined ones
        Assert.Equal(expected, JsonSerializer.Deserialize<TestFlags>(json, options));
    }

    [Theory]
    [InlineData("8")]
    [InlineData("11")]
    [InlineData("-1")]
    [InlineData("\"8\"")]
    public void Deserializes_AValueWithABitThatIsNotDefined_ForAFlagsEnum_Throws(string json)
    {
        Assert.Throws<InvalidCastException>(() => JsonSerializer.Deserialize<TestFlags>(json, options));
    }

    [Fact]
    public void Serializes_ACombinationOfTheValuesOfAFlagsEnum_AsNumber()
    {
        Assert.Equal("3", JsonSerializer.Serialize(TestFlags.Table | TestFlags.View, options));
    }

    [Theory]
    [InlineData("3")]
    [InlineData("9")]
    [InlineData("11")]
    [InlineData("10")]
    public void Deserializes_ACombinationOfTheBitsOfTheNamedValues_OfAFlagsEnum(string json)
    {
        Assert.Equal((TestGroupedFlags)int.Parse(json), JsonSerializer.Deserialize<TestGroupedFlags>(json, options));
    }

    [Fact]
    public void Deserializes_AValueWithABitThatIsNotInTheNamedValues_OfAFlagsEnum_Throws()
    {
        Assert.Throws<InvalidCastException>(() => JsonSerializer.Deserialize<TestGroupedFlags>("4", options));
    }

    [Fact]
    public void Deserializes_AnEnumThatIsNotFlags_OnlyAsADefinedValue()
    {
        // a sum of the values of an enum that is not flags is not a value of it
        Assert.Throws<InvalidCastException>(() => JsonSerializer.Deserialize<TestEnum>("3", options));
    }

    [Fact]
    public void Deserializes_ACombinationOfTheValuesOfAFlagsEnum_OfAnUnsignedType()
    {
        Assert.Equal(TestUnsignedFlags.Low, JsonSerializer.Deserialize<TestUnsignedFlags>("1", options));
        Assert.Throws<InvalidCastException>(() => JsonSerializer.Deserialize<TestUnsignedFlags>("2", options));
    }
}
