namespace Serenity.Data;

public class SqlCollationMapperTests
{
    private static SqlCollationSpec Spec(string comparer)
    {
        Assert.True(SqlCollationSpec.TryParse(comparer, out var spec));
        return spec;
    }

    [Theory]
    [InlineData("Ordinal", null, false, false)]
    [InlineData("ordinalignorecase", null, true, false)]
    [InlineData("InvariantCulture", "", false, false)]
    [InlineData("InvariantCultureIgnoreCase", "", true, false)]
    [InlineData("tr-TR", "tr-TR", false, false)]
    [InlineData("tr-TR:IgnoreCase", "tr-TR", true, false)]
    [InlineData("tr-TR:IgnoreAccents", "tr-TR", false, true)]
    [InlineData(" tr-TR : IgnoreCase : IgnoreAccents ", "tr-TR", true, true)]
    public void Spec_Parses_Comparers(string comparer, string? culture, bool ignoreCase, bool ignoreAccents)
    {
        Assert.Equal(new SqlCollationSpec(culture, ignoreCase, ignoreAccents), Spec(comparer));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("CurrentCulture")]
    [InlineData("CurrentCultureIgnoreCase")]
    [InlineData("tr-TR:Unknown")]
    public void Spec_Rejects_Comparers_That_Cant_Be_Mapped(string? comparer)
    {
        Assert.False(SqlCollationSpec.TryParse(comparer, out _));
    }

    [Theory]
    [InlineData("SqlServer", "Ordinal", "Latin1_General_100_BIN2")]
    [InlineData("SqlServer", "OrdinalIgnoreCase", "Latin1_General_100_CI_AS")]
    [InlineData("SqlServer", "InvariantCultureIgnoreCase", "Latin1_General_100_CI_AS")]
    [InlineData("SqlServer", "tr-TR", "Turkish_100_CS_AS")]
    [InlineData("SqlServer", "tr-TR:IgnoreCase", "Turkish_100_CI_AS")]
    [InlineData("SqlServer", "tr:IgnoreCase:IgnoreAccents", "Turkish_100_CI_AI")]
    [InlineData("SqlServer", "de-DE:IgnoreCase", "Latin1_General_100_CI_AS")]
    [InlineData("SqlServer", "zh-TW", "Chinese_Traditional_Stroke_Count_100_CS_AS")]
    [InlineData("Postgres", "Ordinal", "C")]
    [InlineData("Postgres", "InvariantCulture", "und-x-icu")]
    [InlineData("Postgres", "tr-TR", "tr-TR-x-icu")]
    [InlineData("MySql", "Ordinal", "utf8mb4_bin")]
    [InlineData("MySql", "OrdinalIgnoreCase", "utf8mb4_0900_as_ci")]
    [InlineData("MySql", "tr-TR", "utf8mb4_tr_0900_as_cs")]
    [InlineData("MySql", "tr-TR:IgnoreCase:IgnoreAccents", "utf8mb4_tr_0900_ai_ci")]
    [InlineData("MySql", "en-US:IgnoreCase", "utf8mb4_0900_as_ci")]
    [InlineData("Firebird", "Ordinal", "UCS_BASIC")]
    [InlineData("Firebird", "OrdinalIgnoreCase", "UNICODE_CI")]
    public void Map_Finds_Exact_Database_Collations(string serverType, string comparer, string expected)
    {
        Assert.Equal(expected, SqlCollationMapper.Map(serverType, Spec(comparer), SqlCollationTarget.Database));
    }

    [Theory]
    [InlineData("Sqlite", "Ordinal", "BINARY")]
    [InlineData("Sqlite", "OrdinalIgnoreCase", "NOCASE")]
    [InlineData("MySql", "Ordinal", "utf8_bin")]
    [InlineData("SqlServer", "tr-TR:IgnoreCase", "Turkish_100_CI_AS")]
    [InlineData("Postgres", "tr-TR", "tr-TR-x-icu")]
    public void Map_Finds_Exact_Column_Collations(string serverType, string comparer, string expected)
    {
        Assert.Equal(expected, SqlCollationMapper.Map(serverType, Spec(comparer), SqlCollationTarget.Column));
    }

    [Theory]
    [InlineData("Postgres", "OrdinalIgnoreCase", "C")]
    [InlineData("Postgres", "tr-TR:IgnoreCase", "tr-TR-x-icu")]
    [InlineData("Postgres", "tr-TR:IgnoreCase:IgnoreAccents", "tr-TR-x-icu")]
    [InlineData("MySql", "tr-TR:IgnoreCase", "utf8mb4_tr_0900_ai_ci")]
    public void Tolerant_Falls_Back_When_Only_Case_Or_Accents_Differ(string serverType, string comparer, string expected)
    {
        var spec = Spec(comparer);

        Assert.Throws<NotSupportedException>(() => SqlCollationMapper.Map(serverType, spec, SqlCollationTarget.Database));
        Assert.Null(SqlCollationMapper.TryMap(serverType, spec, SqlCollationTarget.Database));

        Assert.Equal(expected, SqlCollationMapper.Map(serverType, spec, SqlCollationTarget.Database, tolerant: true));
        Assert.Equal(expected, SqlCollationMapper.TryMap(serverType, spec, SqlCollationTarget.Database, tolerant: true));
    }

    [Theory]
    [InlineData("SqlServer", "sw-KE")]
    [InlineData("MySql", "sw-KE")]
    [InlineData("Firebird", "tr-TR")]
    [InlineData("Sqlite", "tr-TR")]
    [InlineData("Oracle", "tr-TR")]
    [InlineData("Oracle", "Ordinal")]
    [InlineData("Unknown", "Ordinal")]
    public void Tolerant_Does_Not_Help_When_There_Is_No_Collation(string serverType, string comparer)
    {
        var spec = Spec(comparer);
        foreach (var tolerant in new[] { false, true })
        {
            Assert.Null(SqlCollationMapper.TryMap(serverType, spec, SqlCollationTarget.Database, tolerant));
            Assert.Throws<NotSupportedException>(() =>
                SqlCollationMapper.Map(serverType, spec, SqlCollationTarget.Database, tolerant));
        }
    }

    [Fact]
    public void Sqlite_Has_No_Database_Collation()
    {
        Assert.Null(SqlCollationMapper.TryMap("Sqlite", Spec("Ordinal"), SqlCollationTarget.Database));
    }

    [Fact]
    public void Oracle_Has_No_Column_Collation_As_It_Depends_On_A_Server_Setting()
    {
        Assert.Null(SqlCollationMapper.TryMap("Oracle", Spec("Ordinal"), SqlCollationTarget.Column, tolerant: true));
    }

    [Fact]
    public void Map_Error_Mentions_Tolerant_Only_When_It_Would_Help()
    {
        var approximate = Assert.Throws<NotSupportedException>(() =>
            SqlCollationMapper.Map("Postgres", Spec("OrdinalIgnoreCase"), SqlCollationTarget.Database));
        Assert.Contains("tolerant", approximate.Message);

        var none = Assert.Throws<NotSupportedException>(() =>
            SqlCollationMapper.Map("Oracle", Spec("Ordinal"), SqlCollationTarget.Database));
        Assert.DoesNotContain("tolerant", none.Message);
    }

    [Fact]
    public void Map_Skips_Collations_That_Dont_Exist_On_Server()
    {
        Assert.Equal("utf8mb4_general_ci", SqlCollationMapper.Map("MySql", Spec("OrdinalIgnoreCase"),
            SqlCollationTarget.Database, tolerant: true, exists: name => !name.Contains("0900")));

        Assert.Equal("utf8mb4_turkish_ci", SqlCollationMapper.Map("MySql", Spec("tr-TR:IgnoreCase:IgnoreAccents"),
            SqlCollationTarget.Database, exists: name => !name.Contains("0900")));

        Assert.Equal("tr-x-icu", SqlCollationMapper.Map("Postgres", Spec("tr-TR"),
            SqlCollationTarget.Database, exists: name => name == "tr-x-icu"));

        Assert.Null(SqlCollationMapper.TryMap("SqlServer", Spec("tr-TR"),
            SqlCollationTarget.Database, exists: _ => false));
    }

    [Theory]
    [InlineData("automap", true)]
    [InlineData("AUTOMAP", true)]
    [InlineData("automaptolerant", true)]
    [InlineData("tryautomap", true)]
    [InlineData("tryautomaptolerant", true)]
    [InlineData("Turkish_100_CI_AS", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsAutoMap_Detects_Keywords(string? collation, bool expected)
    {
        Assert.Equal(expected, SqlCollationMapper.IsAutoMap(collation));
    }

    [Theory]
    [InlineData("automap", "Turkish_100_CI_AS")]
    [InlineData("automaptolerant", "Turkish_100_CI_AS")]
    [InlineData("tryautomap", "Turkish_100_CI_AS")]
    [InlineData("tryautomaptolerant", "Turkish_100_CI_AS")]
    public void ResolveAutoMap_Maps_Exact_In_All_Modes(string keyword, string expected)
    {
        Assert.Equal(expected, SqlCollationMapper.ResolveAutoMap(keyword, "tr-TR:IgnoreCase", "SqlServer",
            SqlCollationTarget.Database));
    }

    [Fact]
    public void ResolveAutoMap_Case_Or_Accent_Difference_Depends_On_Mode()
    {
        const string comparer = "tr-TR:IgnoreCase";
        const string server = "Postgres";
        const SqlCollationTarget target = SqlCollationTarget.Database;

        Assert.Throws<NotSupportedException>(() => SqlCollationMapper.ResolveAutoMap("automap", comparer, server, target));
        Assert.Equal("tr-TR-x-icu", SqlCollationMapper.ResolveAutoMap("automaptolerant", comparer, server, target));
        Assert.Null(SqlCollationMapper.ResolveAutoMap("tryautomap", comparer, server, target));
        Assert.Equal("tr-TR-x-icu", SqlCollationMapper.ResolveAutoMap("tryautomaptolerant", comparer, server, target));
    }

    [Theory]
    [InlineData("automap")]
    [InlineData("automaptolerant")]
    public void ResolveAutoMap_Unmapped_Culture_Throws_For_Non_Try_Modes(string keyword)
    {
        Assert.Throws<NotSupportedException>(() => SqlCollationMapper.ResolveAutoMap(keyword, "sw-KE", "SqlServer",
            SqlCollationTarget.Database));
    }

    [Theory]
    [InlineData("tryautomap")]
    [InlineData("tryautomaptolerant")]
    public void ResolveAutoMap_Unmapped_Culture_Returns_Null_For_Try_Modes(string keyword)
    {
        Assert.Null(SqlCollationMapper.ResolveAutoMap(keyword, "sw-KE", "SqlServer", SqlCollationTarget.Database));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("CurrentCulture")]
    public void ResolveAutoMap_Without_A_Fixed_Comparer_Depends_On_Mode(string? comparer)
    {
        Assert.Throws<NotSupportedException>(() => SqlCollationMapper.ResolveAutoMap("automaptolerant", comparer, "SqlServer",
            SqlCollationTarget.Database));
        Assert.Null(SqlCollationMapper.ResolveAutoMap("tryautomaptolerant", comparer, "SqlServer",
            SqlCollationTarget.Database));
    }

    [Fact]
    public void ComparerResolver_Accepts_IgnoreAccents()
    {
        var comparer = ComparerResolver.Parse("en-US:IgnoreCase:IgnoreAccents").AssertNotNull();
        Assert.Equal(0, comparer.Compare("CAFE", "café"));

        var accents = ComparerResolver.Parse("en-US:IgnoreAccents").AssertNotNull();
        Assert.Equal(0, accents.Compare("cafe", "café"));
        Assert.NotEqual(0, accents.Compare("CAFE", "cafe"));
    }
}
