namespace Serenity.Data;

public class SqlConversionsTests
{
    [Fact]
    public void NullConstant_IsNullText() => Assert.Equal("NULL", SqlConversions.Null);

    [Fact]
    public void ToSql_BoolNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((bool?)null).ToSql());
    }

    [Theory]
    [InlineData(true, "1")]
    [InlineData(false, "0")]
    public void ToSql_Bool_ReturnsOneOrZero(bool value, string expected)
    {
        Assert.Equal(expected, ((bool?)value).ToSql());
    }

    [Fact]
    public void ToSql_DoubleNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((double?)null).ToSql());
    }

    [Fact]
    public void ToSql_Double_UsesInvariantFormatting()
    {
        Assert.Equal("1.5", ((double?)1.5).ToSql());
    }

    [Fact]
    public void ToSql_DecimalNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((decimal?)null).ToSql());
    }

    [Fact]
    public void ToSql_Decimal_UsesInvariantFormatting()
    {
        Assert.Equal("2.25", ((decimal?)2.25m).ToSql());
    }

    [Fact]
    public void ToSql_LongNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((long?)null).ToSql());
    }

    [Fact]
    public void ToSql_Long_UsesInvariantFormatting()
    {
        Assert.Equal("123456789012", ((long?)123456789012L).ToSql());
    }

    [Fact]
    public void ToSql_IntNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((int?)null).ToSql());
    }

    [Fact]
    public void ToSql_Int_UsesInvariantFormatting()
    {
        Assert.Equal("42", ((int?)42).ToSql());
    }

    [Fact]
    public void ToSql_DateTimeNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((DateTime?)null).ToSql());
    }

    [Fact]
    public void ToSql_DateOnlyDateTime_UsesDateFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;

        Assert.Equal(new DateTime(2023, 5, 1).ToSql(dialect), ((DateTime?)new DateTime(2023, 5, 1)).ToSql(dialect));
    }

    [Fact]
    public void ToSql_DateTimeWithTime_UsesDateTimeFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.DateTimeFormat, Invariants.DateTimeFormat),
            ((DateTime?)value).ToSql(dialect));
    }

    [Fact]
    public void ToSql_DateTime_NonNullable_WithTime_UsesDateTimeFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.DateTimeFormat, Invariants.DateTimeFormat), value.ToSql(dialect));
    }

    [Fact]
    public void ToSqlDate_NullableNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((DateTime?)null).ToSqlDate());
    }

    [Fact]
    public void ToSqlDate_Nullable_UsesDateFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.DateFormat, Invariants.DateTimeFormat), ((DateTime?)value).ToSqlDate(dialect));
    }

    [Fact]
    public void ToSqlDate_NonNullable_UsesDateFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.DateFormat, Invariants.DateTimeFormat), value.ToSqlDate(dialect));
    }

    [Fact]
    public void ToSqlTime_NullableNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((DateTime?)null).ToSqlTime());
    }

    [Fact]
    public void ToSqlTime_Nullable_UsesTimeFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.TimeFormat, Invariants.DateTimeFormat), ((DateTime?)value).ToSqlTime(dialect));
    }

    [Fact]
    public void ToSqlTime_NonNullable_UsesTimeFormat()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var value = new DateTime(2023, 5, 1, 12, 30, 45);

        Assert.Equal(value.ToString(dialect.TimeFormat, Invariants.DateTimeFormat), value.ToSqlTime(dialect));
    }

    [Fact]
    public void ToSql_GuidNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((Guid?)null).ToSql());
    }

    [Fact]
    public void ToSql_GuidValue_QuotedFormat()
    {
        var guid = Guid.NewGuid();

        Assert.Equal("'" + guid.ToString("D") + "'", ((Guid?)guid).ToSql());
    }

    [Fact]
    public void ToSql_StringNull_ReturnsNullConstant()
    {
        Assert.Equal("NULL", ((string?)null).ToSql());
    }

    [Fact]
    public void ToSql_String_QuotesValue()
    {
        Assert.Equal("N'abc'", "abc".ToSql(SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void Translate_String_WithConnection_DialectTranslation()
    {
        var connection = new MockDbConnection { Dialect = OracleDialect.Instance };

        var result = SqlConversions.Translate("SELECT A FROM [Table] WHERE X = @p1", connection);

        Assert.Equal("SELECT A FROM \"TABLE\" WHERE X = :p1", result);
    }

    [Fact]
    public void Translate_Dialect_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SqlConversions.Translate("SELECT 1", (ISqlDialect)null));
    }

    [Fact]
    public void Translate_Query_WithConnection_TranslatesQueryToString()
    {
        var connection = new MockDbConnection { Dialect = OracleDialect.Instance };
        var query = new SqlQuery().Dialect(SqlServer2012Dialect.Instance)
            .Select("Name").From("[T]").Where(new Criteria("X") == 1);

        var result = SqlConversions.Translate(query, connection);

        Assert.Contains("T", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Translate_Query_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            SqlConversions.Translate((IQueryWithParams)null, new MockDbConnection()));
    }
}
