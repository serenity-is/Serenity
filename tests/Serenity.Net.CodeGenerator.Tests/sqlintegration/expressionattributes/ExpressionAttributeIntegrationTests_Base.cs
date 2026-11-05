using System.Globalization;

namespace Serenity.Data.Mapping;

/// <summary>
/// Runs the expressions of the expression attributes on a real database, expecting the results of SQL Server, whose
/// syntax and meaning they follow, e.g. DATEDIFF counts the boundaries crossed between two dates, not complete units.
/// Values are passed as parameters, as they are when the expressions are used with criteria.
/// </summary>
public abstract class ExpressionAttributeIntegrationTests_Base
{
    /// <summary>
    /// Provider name passed to <see cref="SqlIntegrationConnections.CreateConnection(string)"/>.
    /// </summary>
    protected abstract string Provider { get; }

    /// <summary>
    /// The dialect of the provider.
    /// </summary>
    protected abstract ISqlDialect Dialect { get; }

    /// <summary>
    /// The table to select a single row from, for databases that require one, like DUAL for Oracle.
    /// </summary>
    protected virtual string? From => null;

    private object? Scalar(BaseExpressionAttribute expression, params (string Name, object? Value)[] parameters)
    {
        using var actual = SqlIntegrationConnections.CreateConnection(Provider);
        using var connection = new WrappedConnection(actual, Dialect);

        var query = new SqlQuery().Dialect(Dialect).Select(expression.ToString(Dialect), "V");
        if (From is not null)
            query.From(From);

        foreach (var (name, value) in parameters)
            query.AddParam(name, value);

        var result = SqlHelper.ExecuteScalar(connection, query);
        return result is DBNull ? null : result;
    }

    private static DateTime Date(string value) => DateTime.Parse(value, CultureInfo.InvariantCulture);

    private static DateTime ToDateTime(object? value) => value switch
    {
        DateTime dateTime => dateTime,
        DateTimeOffset offset => offset.UtcDateTime,
        string text => DateTime.Parse(text, CultureInfo.InvariantCulture),
        _ => throw new InvalidOperationException($"Unexpected {value?.GetType().Name ?? "null"} for a date.")
    };

    [Theory]
    [InlineData(DateParts.Year, 2026)]
    [InlineData(DateParts.Month, 3)]
    [InlineData(DateParts.Day, 15)]
    [InlineData(DateParts.Hour, 10)]
    [InlineData(DateParts.Minute, 30)]
    [InlineData(DateParts.Second, 45)]
    public void DatePart_ReturnsThePart(DateParts part, int expected)
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new DatePartAttribute(part, "@d"), ("@d", Date("2026-03-15 10:30:45")));

        Assert.Equal(expected, Convert.ToInt64(value, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(DateParts.Year, "2025-12-31 23:59:59", "2026-01-01 00:00:00", 1)]
    [InlineData(DateParts.Year, "2026-01-01 00:00:00", "2026-12-31 23:59:59", 0)]
    [InlineData(DateParts.Year, "2026-06-01 00:00:00", "2024-07-01 00:00:00", -2)]
    [InlineData(DateParts.Month, "2026-01-31 00:00:00", "2026-02-01 00:00:00", 1)]
    [InlineData(DateParts.Month, "2025-11-15 00:00:00", "2026-02-14 00:00:00", 3)]
    [InlineData(DateParts.Day, "2026-03-14 23:00:00", "2026-03-15 01:00:00", 1)]
    [InlineData(DateParts.Day, "2026-03-15 01:00:00", "2026-03-15 23:00:00", 0)]
    [InlineData(DateParts.Day, "2026-02-27 12:00:00", "2026-03-02 08:00:00", 3)]
    [InlineData(DateParts.Day, "2026-03-15 00:00:00", "2026-03-10 00:00:00", -5)]
    [InlineData(DateParts.Hour, "2026-03-15 10:59:00", "2026-03-15 11:01:00", 1)]
    [InlineData(DateParts.Hour, "2026-03-15 10:01:00", "2026-03-15 10:59:00", 0)]
    [InlineData(DateParts.Hour, "2026-03-14 23:30:00", "2026-03-15 01:10:00", 2)]
    [InlineData(DateParts.Minute, "2026-03-15 10:00:59", "2026-03-15 10:01:01", 1)]
    [InlineData(DateParts.Minute, "2026-03-15 10:00:01", "2026-03-15 10:00:59", 0)]
    [InlineData(DateParts.Second, "2026-03-15 10:00:00", "2026-03-15 10:00:05", 5)]
    [InlineData(DateParts.Second, "2026-03-15 10:00:05", "2026-03-15 09:59:55", -10)]
    public void DateDiff_CountsTheBoundariesCrossed(DateParts part, string start, string end, int expected)
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new DateDiffAttribute(part, "@d1", "@d2"), ("@d1", Date(start)), ("@d2", Date(end)));

        Assert.Equal(expected, Convert.ToInt64(value, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(DateParts.Year, 1, "2026-03-15 10:30:00", "2027-03-15 10:30:00")]
    [InlineData(DateParts.Year, 1, "2024-02-29 10:30:00", "2025-02-28 10:30:00")]
    [InlineData(DateParts.Year, -2, "2026-03-15 10:30:00", "2024-03-15 10:30:00")]
    [InlineData(DateParts.Month, 1, "2026-01-31 10:30:00", "2026-02-28 10:30:00")]
    [InlineData(DateParts.Month, 1, "2026-02-28 10:30:00", "2026-03-28 10:30:00")]
    [InlineData(DateParts.Month, -1, "2026-03-31 10:30:00", "2026-02-28 10:30:00")]
    [InlineData(DateParts.Month, 14, "2026-03-15 10:30:00", "2027-05-15 10:30:00")]
    [InlineData(DateParts.Day, 10, "2026-03-25 10:30:00", "2026-04-04 10:30:00")]
    [InlineData(DateParts.Day, -15, "2026-03-10 10:30:00", "2026-02-23 10:30:00")]
    [InlineData(DateParts.Hour, 25, "2026-03-15 10:30:00", "2026-03-16 11:30:00")]
    [InlineData(DateParts.Minute, 90, "2026-03-15 10:30:00", "2026-03-15 12:00:00")]
    [InlineData(DateParts.Second, -3601, "2026-03-15 10:30:00", "2026-03-15 09:29:59")]
    public void DateAdd_AddsToTheDate(DateParts part, int number, string date, string expected)
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new DateAddAttribute(part, "@n", "@d"), ("@n", number), ("@d", Date(date)));

        Assert.Equal(Date(expected), ToDateTime(value));
    }

    [Fact]
    public void DateAdd_AcceptsTheResultOfAnotherExpression()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new DateAddAttribute(DateParts.Day, "@n",
            new DateAddAttribute(DateParts.Month, "@n", "@d").ToString(Dialect)), ("@n", 1), ("@d", Date("2026-01-31 00:00:00")));

        Assert.Equal(Date("2026-03-01 00:00:00"), ToDateTime(value));
    }

    [Fact]
    public void SqlUtcNow_IsTheCurrentUtcTime()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = ToDateTime(Scalar(new SqlUtcNowAttribute()));

        Assert.InRange(value, DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
    }

    [Fact]
    public void SqlNow_IsADate()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = ToDateTime(Scalar(new SqlNowAttribute()));

        // the server may be in any time zone
        Assert.InRange(value, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
    }

    [Fact]
    public void SqlDateTimeOffset_IsTheCurrentTime()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        Assert.NotNull(Scalar(new SqlDateTimeOffsetAttribute()));
    }

    [Fact]
    public void Concat_TreatsNullAsEmpty()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new ConcatAttribute("@a", "NULL", "@b"), ("@a", "x"), ("@b", "y"));

        Assert.Equal("xy", value);
    }

    [Fact]
    public void CaseSwitch_ReturnsTheMatchingBranch()
    {
        if (SqlIntegrationConnections.ShouldSkip(Provider))
            return;

        var value = Scalar(new CaseSwitchAttribute("@n", "1", "'one'", "2", "'two'", "'other'"), ("@n", 2));

        // Firebird pads string literals to the longest one of the CASE, as they are CHAR
        Assert.Equal("two", (value as string)?.TrimEnd());
    }
}
