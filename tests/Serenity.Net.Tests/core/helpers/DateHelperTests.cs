using System.Globalization;

namespace Serenity;

public class DateHelperTests
{
    [Fact]
    public void TryParseISO8601DateTime_Parses_DateOnly()
    {
        Assert.True("2024-01-15".TryParseISO8601DateTime(out var date));
        Assert.Equal(new DateTime(2024, 1, 15), date);
    }

    [Fact]
    public void TryParseISO8601DateTime_Parses_DateTime()
    {
        Assert.True("2024-01-15T10:30:00".TryParseISO8601DateTime(out var date));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0), date);
    }

    [Fact]
    public void TryParseISO8601DateTime_Parses_DateTimeWithMilliseconds()
    {
        Assert.True("2024-01-15T10:30:00.123".TryParseISO8601DateTime(out var date));
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 0, 123), date);
    }

    [Fact]
    public void TryParseISO8601DateTime_Parses_UtcSuffix()
    {
        Assert.True("2024-01-15T10:30:00Z".TryParseISO8601DateTime(out var date));
        Assert.Equal(2024, date.Year);
        Assert.Equal(1, date.Month);
        Assert.Equal(15, date.Day);
    }

    [Fact]
    public void TryParseISO8601DateTime_ReturnsFalse_ForInvalidString()
    {
        Assert.False("not-a-date".TryParseISO8601DateTime(out _));
        Assert.False("".TryParseISO8601DateTime(out _));
        Assert.False(((string)null).TryParseISO8601DateTime(out _));
    }

    [Theory]
    [InlineData("dd/MM/yyyy", DateElementOrder.DayMonthYear)]
    [InlineData("MM/dd/yyyy", DateElementOrder.MonthDayYear)]
    [InlineData("yyyy/MM/dd", DateElementOrder.YearMonthDay)]
    public void DateElementOrderFor_ReturnsExpectedOrder(string pattern, DateElementOrder expected)
    {
        Assert.Equal(expected, DateHelper.DateElementOrderFor(pattern));
    }

    [Fact]
    public void DateOrderString_ReturnsExpectedStrings()
    {
        Assert.Equal("dmy", DateHelper.DateOrderString(DateElementOrder.DayMonthYear));
        Assert.Equal("mdy", DateHelper.DateOrderString(DateElementOrder.MonthDayYear));
        Assert.Equal("ymd", DateHelper.DateOrderString(DateElementOrder.YearMonthDay));
    }

    [Fact]
    public void DefaultDateFormat_ReturnsExpectedFormats()
    {
        Assert.Equal("dd/MM/yyyy", DateHelper.DefaultDateFormat(DateElementOrder.DayMonthYear));
        Assert.Equal("MM/dd/yyyy", DateHelper.DefaultDateFormat(DateElementOrder.MonthDayYear));
        Assert.Equal("yyyy/MM/dd", DateHelper.DefaultDateFormat(DateElementOrder.YearMonthDay));
    }

    [Fact]
    public void DefaultDateTimeFormat_ReturnsExpectedFormats()
    {
        Assert.Equal("dd/MM/yyyy HH:mm:ss", DateHelper.DefaultDateTimeFormat(DateElementOrder.DayMonthYear));
        Assert.Equal("MM/dd/yyyy HH:mm:ss", DateHelper.DefaultDateTimeFormat(DateElementOrder.MonthDayYear));
        Assert.Equal("yyyy/MM/dd HH:mm:ss", DateHelper.DefaultDateTimeFormat(DateElementOrder.YearMonthDay));
    }

    [Fact]
    public void CurrentDateFormat_ReturnsFormatForCurrentCulture()
    {
        var expected = DateHelper.DefaultDateFormat(DateHelper.CurrentDateElementOrder);
        Assert.Equal(expected, DateHelper.CurrentDateFormat);
    }

    [Fact]
    public void CurrentDateTimeFormat_ReturnsFormatForCurrentCulture()
    {
        var expected = DateHelper.DefaultDateTimeFormat(DateHelper.CurrentDateElementOrder);
        Assert.Equal(expected, DateHelper.CurrentDateTimeFormat);
    }

    [Fact]
    public void ISODateTimeFormatUTC_IsValid()
    {
        Assert.Equal("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'", DateHelper.ISODateTimeFormatUTC);
    }

    [Fact]
    public void ISODateTimeFormatLocal_IsValid()
    {
        Assert.Equal("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", DateHelper.ISODateTimeFormatLocal);
    }
}
