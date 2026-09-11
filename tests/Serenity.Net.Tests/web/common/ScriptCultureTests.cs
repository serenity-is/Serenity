using System.Globalization;

namespace Serenity;

public class ScriptCultureTests
{
    [Fact]
    public void Constructor_Uses_CurrentCulture_ByDefault()
    {
        var culture = new ScriptCulture();
        var expected = new ScriptCulture(CultureInfo.CurrentCulture);

        Assert.Equal(expected.DateOrder, culture.DateOrder);
        Assert.Equal(expected.DateFormat, culture.DateFormat);
        Assert.Equal(expected.DateTimeFormat, culture.DateTimeFormat);
        Assert.Equal(expected.DateSeparator, culture.DateSeparator);
        Assert.Equal(expected.DecimalSeparator, culture.DecimalSeparator);
        Assert.Equal(expected.GroupSeparator, culture.GroupSeparator);
    }

    [Theory]
    [InlineData("en-US", "mdy", "MM/dd/yyyy", "MM/dd/yyyy HH:mm:ss", "/", ".", ",")]
    [InlineData("de-DE", "dmy", "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", ".", ",", ".")]
    [InlineData("ja-JP", "ymd", "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "/", ".", ",")]
    public void Constructor_Reads_Culture_Settings(string cultureName, string dateOrder,
        string dateFormat, string dateTimeFormat, string dateSeparator,
        string decimalSeparator, string groupSeparator)
    {
        var culture = new ScriptCulture(new CultureInfo(cultureName));

        Assert.Equal(dateOrder, culture.DateOrder);
        Assert.Equal(dateFormat, culture.DateFormat);
        Assert.Equal(dateTimeFormat, culture.DateTimeFormat);
        Assert.Equal(dateSeparator, culture.DateSeparator);
        Assert.Equal(decimalSeparator, culture.DecimalSeparator);
        Assert.Equal(groupSeparator, culture.GroupSeparator);
    }

    [Fact]
    public void Constructor_FallsBack_When_Culture_Has_Empty_DateSeparator()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.DateTimeFormat.DateSeparator = "";
        var expected = DateTime.Now.ToString("yy/MM/dd", culture.DateTimeFormat)[2].ToString();

        Assert.Equal(expected, new ScriptCulture(culture).DateSeparator);
    }

    [Fact]
    public void Properties_Are_Settable()
    {
        var culture = new ScriptCulture(new CultureInfo("en-US"))
        {
            DateOrder = "a",
            DateFormat = "b",
            DateTimeFormat = "c",
            DateSeparator = "d",
            DecimalSeparator = "e",
            GroupSeparator = "f"
        };

        Assert.Equal("a", culture.DateOrder);
        Assert.Equal("b", culture.DateFormat);
        Assert.Equal("c", culture.DateTimeFormat);
        Assert.Equal("d", culture.DateSeparator);
        Assert.Equal("e", culture.DecimalSeparator);
        Assert.Equal("f", culture.GroupSeparator);
    }
}
