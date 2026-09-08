using System.Globalization;

namespace Serenity;

public class InvariantsTests
{
    [Fact]
    public void NumberFormat_IsInvariant()
    {
        Assert.Same(NumberFormatInfo.InvariantInfo, Invariants.NumberFormat);
    }

    [Fact]
    public void DateTimeFormat_IsInvariant()
    {
        Assert.Same(DateTimeFormatInfo.InvariantInfo, Invariants.DateTimeFormat);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(1L, true)]
    [InlineData((short)1, true)]
    [InlineData(1.5, false)]
    [InlineData("1", false)]
    [InlineData(null, false)]
    public void IsIntegerType_ReturnsExpected(object? value, bool expected)
    {
        Assert.Equal(expected, Invariants.IsIntegerType(value));
    }

    [Fact]
    public void ToInvariant_Int_ReturnsInvariantString()
    {
        Assert.Equal("12345", 12345.ToInvariant());
    }

    [Fact]
    public void ToInvariant_Long_ReturnsInvariantString()
    {
        Assert.Equal("123456789", 123456789L.ToInvariant());
    }

    [Fact]
    public void ToInvariant_Double_ReturnsInvariantString()
    {
        Assert.Equal("3.14", 3.14.ToInvariant());
    }

    [Fact]
    public void ToInvariant_Decimal_ReturnsInvariantString()
    {
        Assert.Equal("3.14", 3.14m.ToInvariant());
    }

    [Theory]
    [InlineData("42", 42L)]
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("abc", null)]
    public void TryParseID_ReturnsExpected(string? input, long? expected)
    {
        Assert.Equal(expected, input.TryParseID());
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("abc", null)]
    public void TryParseID32_ReturnsExpected(string? input, int? expected)
    {
        Assert.Equal(expected, input.TryParseID32());
    }

    [Fact]
    public void IDString_ReturnsEmpty_WhenNull()
    {
        Assert.Equal(string.Empty, ((long?)null).IDString());
    }

    [Fact]
    public void IDString_ReturnsValue_WhenNotNull()
    {
        Assert.Equal("42", ((long?)42).IDString());
    }
}
