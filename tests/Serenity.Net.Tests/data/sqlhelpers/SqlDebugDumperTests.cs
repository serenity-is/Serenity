namespace Serenity.Data;

public class SqlDebugDumperTests
{
    private static Dictionary<string, object?> MakePrm(string key, object? value) =>
        new() { [key] = value };

    [Fact]
    public void Dump_ReturnsSqlAsIs()
    {
        Assert.Equal("SELECT 1", SqlDebugDumper.Dump("SELECT 1", null));
    }

    [Fact]
    public void Dump_ReplacesParams()
    {
        var prm0 = new Dictionary<string, object?> { ["@p1"] = 1, ["@p2"] = "x" };
        var result = SqlDebugDumper.Dump("SELECT * FROM T WHERE A = @p1 AND B = @p2", prm0);

        Assert.Equal("SELECT * FROM T WHERE A = 1 AND B = N'x'", result);
    }

    [Fact]
    public void Dump_AddsAtPrefixToParameterNames()
    {
        var prm = MakePrm("p1", 5);

        var result = SqlDebugDumper.Dump("SELECT * FROM T WHERE A = @p1", prm);

        Assert.Equal("SELECT * FROM T WHERE A = 5", result);
    }

    [Fact]
    public void Dump_ReplacesLongestParameterFirst()
    {
        var prm = new Dictionary<string, object?> { ["@p1"] = 1, ["@p11"] = 11 };

        var result = SqlDebugDumper.Dump("WHERE X = @p1 AND Y = @p11", prm);

        Assert.Equal("WHERE X = 1 AND Y = 11", result);
    }

    [Fact]
    public void Dump_ParameterValues_Conversion()
    {
        Assert.Equal("X = NULL", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", null)));
        Assert.Equal("X = NULL", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", DBNull.Value)));
        Assert.Equal("X = N'a'", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", 'a')));
        Assert.Equal("X = N'System.Char[]'", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", "ab".ToCharArray())));
        Assert.Equal("X = 1", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", true)));
        Assert.Equal("X = 0", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", false)));
    }

    [Fact]
    public void Dump_ParameterValueDateTime_UsesSqlFormats()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var date = new DateTime(2023, 5, 1);
        var dateTime = new DateTime(2023, 5, 1, 12, 0, 0);

        Assert.Equal("X = " + date.ToSqlDate(dialect), SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", date), dialect));
        Assert.Equal("X = " + dateTime.ToSql(dialect), SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", dateTime), dialect));
    }

    [Fact]
    public void Dump_ParameterValueDateTimeOffset_Quoted()
    {
        var dto = new DateTimeOffset(2023, 5, 1, 12, 0, 0, TimeSpan.Zero);

        Assert.Equal("X = '" + dto.ToString("o") + "'", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", dto)));
    }

    [Fact]
    public void Dump_ParameterValueGuid_Quoted()
    {
        var guid = Guid.NewGuid();

        Assert.Equal("X = '" + guid + "'", SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", guid)));
    }

    [Fact]
    public void Dump_ParameterValueByteArray_ThrowsDueToInvalidH2Format()
    {
        Assert.Throws<FormatException>(() =>
            SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", new byte[] { 1, 2, 10 })));
    }

    [Fact]
    public void Dump_ParameterValueMemoryStream_ThrowsDueToInvalidH2Format()
    {
        var ms = new System.IO.MemoryStream([1, 3]);

        Assert.Throws<FormatException>(() =>
            SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", ms)));
    }

    [Fact]
    public void Dump_ParameterValueIFormattable_UsesInvariant()
    {
        var result = SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", (object)5.5d));

        Assert.Equal("X = 5.5", result);
    }

    [Fact]
    public void Dump_ParameterValueOtherType_UsesToString()
    {
        var value = new SqlDebugDumperTests();

        var result = SqlDebugDumper.Dump("X = @p1", MakePrm("@p1", value));

        Assert.Equal("X = " + value.ToString(), result);
    }

    [Fact]
    public void Dump_WithDialect_TranslatesBrackets()
    {
        var dialect = PostgresDialect.Instance;

        var result = SqlDebugDumper.Dump("SELECT A FROM [Table] WHERE A = @p1", MakePrm("@p1", 1), dialect);

        Assert.Equal("SELECT A FROM \"Table\" WHERE A = 1", result);
    }

    [Fact]
    public void Dump_WithDialectUsingNonAtPrefix_TranslatesParameterPrefix()
    {
        var dialect = OracleDialect.Instance;

        var result = SqlDebugDumper.Dump("SELECT A FROM [Table] WHERE A = @p1", MakePrm("@p1", "@p2"), dialect);

        Assert.Contains("\"TABLE\"", result, StringComparison.Ordinal);
        Assert.Contains("'@p2'", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Dump_Replaces_DatabaseCaret_References()
    {
        var old = DatabaseCaretReferences.SetLocalGetDatabaseName(_ => "Db1.dbo");
        try
        {
            var result = SqlDebugDumper.Dump("SELECT [Db1^Table].[Table].[A]", new Dictionary<string, object?>());

            Assert.Contains("[Db1.dbo].[Table]", result, StringComparison.Ordinal);
        }
        finally
        {
            DatabaseCaretReferences.SetLocalGetDatabaseName(old);
        }
    }

    [Fact]
    public void Dump_NullSql_WithParams_Works()
    {
        Assert.Equal("", SqlDebugDumper.Dump(null, MakePrm("@p1", 1)));
    }

    public override string ToString() => "DebugDumperCustomValue";
}
