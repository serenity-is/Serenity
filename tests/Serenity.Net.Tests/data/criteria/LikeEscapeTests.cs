using System.Text.Json;

namespace Serenity.Data;

public class LikeEscapeTests
{
    private sealed class RenameNameVisitor : BaseCriteriaVisitor
    {
        public BaseCriteria Rewrite(BaseCriteria criteria)
        {
            return Visit(criteria)!;
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            if (criteria.Expression == "Name")
                return new Criteria("Renamed");
            return base.VisitCriteria(criteria);
        }
    }

    private static JsonSerializerOptions GetStjOptions()
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        options.Converters.Add(new JsonConverters.CriteriaJsonConverter());
        return options;
    }

    // LikeEscape

    [Theory]
    [InlineData("abc", "abc")]
    [InlineData("", "")]
    [InlineData("50%", "50!%")]
    [InlineData("a_b", "a!_b")]
    [InlineData("a[b", "a![b")]
    [InlineData("a]b", "a]b")] // ] outside [...] is literal everywhere; left as is
    [InlineData("a!b", "a!!b")]
    [InlineData("!", "!!")]
    [InlineData("%_[!", "!%!_![!!")]
    public void LikeEscape_EscapesSpecials(string value, string expected)
    {
        Assert.Equal(expected, Criteria.EscapeLikeWildcards(value));
    }

    [Fact]
    public void LikeEscape_CustomEscapeChar()
    {
        Assert.Equal("a/%b//c", Criteria.EscapeLikeWildcards("a%b/c", '/'));
    }

    [Fact]
    public void LikeEscape_NullValue_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Criteria.EscapeLikeWildcards(null!));
    }

    [Theory]
    [InlineData('%')]
    [InlineData('_')]
    [InlineData('\'')]
    public void LikeEscape_InvalidEscapeChar_ThrowsArgumentException(char escape)
    {
        Assert.Throws<ArgumentException>(() => Criteria.EscapeLikeWildcards("abc", escape));
    }

    // BinaryCriteria ctor

    [Fact]
    public void Constructor_LikeEscapeChar_DefaultsToNull()
    {
        var criteria = new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like, new ValueCriteria("x%"));

        Assert.Null(criteria.LikeEscapeChar);
        Assert.Equal("(A LIKE @p1)", criteria.ToString(new SqlQuery()));
    }

    [Fact]
    public void Constructor_NonLikeOperator_WithEscape_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new BinaryCriteria(new Criteria("A"), CriteriaOperator.EQ, new ValueCriteria("x"), '!'));
    }

    [Theory]
    [InlineData('%')]
    [InlineData('_')]
    [InlineData('\'')]
    public void Constructor_InvalidEscapeChar_ThrowsArgumentException(char escape)
    {
        Assert.Throws<ArgumentException>(() =>
            new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like, new ValueCriteria("x"), escape));
    }

    [Fact]
    public void Constructor_CleanMask_DoesNotRetainEscapeChar()
    {
        // The clause would be inert, so the flag is omitted entirely.
        var criteria = new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like, new ValueCriteria("x%"), '!');

        Assert.Null(criteria.LikeEscapeChar);
        Assert.Equal("(A LIKE @p1)", criteria.ToString(new SqlQuery()));
    }

    [Fact]
    public void Constructor_UpperWrappedCleanMask_DoesNotRetainEscapeChar()
    {
        var criteria = new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like,
            new UpperFunctionCriteria(new ValueCriteria("x%")), '!');

        Assert.Null(criteria.LikeEscapeChar);
    }

    [Fact]
    public void Constructor_UpperWrappedDirtyMask_RetainsEscapeChar()
    {
        var criteria = new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like,
            new UpperFunctionCriteria(new ValueCriteria("%a!%b%")), '!');

        Assert.Equal('!', criteria.LikeEscapeChar);
    }

    [Fact]
    public void Constructor_InvalidEscapeChar_ThrowsEvenForCleanMask()
    {
        // Validation runs before the inert-clause check.
        Assert.Throws<ArgumentException>(() =>
            new BinaryCriteria(new Criteria("A"), CriteriaOperator.Like, new ValueCriteria("abc"), '%'));
    }

    // Rendering

    [Fact]
    public void Contains_RendersLikeWithEscapeAndEscapedParam()
    {
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1 ESCAPE '!')",
            new Criteria("Name").Contains("50%_x").ToString(query));
        Assert.Equal("%50!%!_x%", query.Params["@p1"]);
    }

    [Fact]
    public void StartsWith_RendersLikeWithEscapeAndEscapedParam()
    {
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1 ESCAPE '!')",
            new Criteria("Name").StartsWith("a[b").ToString(query));
        Assert.Equal("a![b%", query.Params["@p1"]);
    }

    [Fact]
    public void EndsWith_RendersLikeWithEscapeAndEscapedParam()
    {
        var query = new SqlQuery();

        // ']' is literal on all dialects: nothing escaped, no ESCAPE clause.
        Assert.Equal("(Name LIKE @p1)",
            new Criteria("Name").EndsWith("a]b").ToString(query));
        Assert.Equal("%a]b", query.Params["@p1"]);
    }

    [Fact]
    public void NotContains_RendersNotLikeWithEscape()
    {
        var query = new SqlQuery();

        Assert.Equal("(Name NOT LIKE @p1 ESCAPE '!')",
            new Criteria("Name").NotContains("a%b").ToString(query));
        Assert.Equal("%a!%b%", query.Params["@p1"]);
    }

    [Fact]
    public void Contains_WithUpper_WrapsBothSidesInUpperFunction()
    {
        var query = new SqlQuery();

        Assert.Equal("(UPPER(Name) LIKE UPPER(@p1) ESCAPE '!')",
            new Criteria("Name").Contains("a%b", upper: true).ToString(query));
        Assert.Equal("%a!%b%", query.Params["@p1"]);
    }

    [Fact]
    public void LikeEscaped_CustomEscapeChar()
    {
        var query = new SqlQuery();

        // Custom escape chars are only available via the pre-escaped Like hatch.
        var mask = "%" + Criteria.EscapeLikeWildcards("a/b", '/') + "%";
        Assert.Equal("(Name LIKE @p1 ESCAPE '/')",
            new Criteria("Name").LikeEscaped(mask, escape: '/').ToString(query));
        Assert.Equal("%a//b%", query.Params["@p1"]);
    }

    [Fact]
    public void LikeEscaped_RendersEscapeWithMaskAsIs()
    {
        var query = new SqlQuery();

        // Pre-escaped mask passes through; the overload only declares ESCAPE.
        Assert.Equal("(Name LIKE @p1 ESCAPE '!')",
            new Criteria("Name").LikeEscaped("a%!_b").ToString(query));
        Assert.Equal("a%!_b", query.Params["@p1"]);
    }

    [Fact]
    public void Like_WithoutEscape_OldOverloadIsUnchanged()
    {
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1)",
            new Criteria("Name").Like("a%").ToString(query));
    }

    [Fact]
    public void AffixMethods_NullValue_ThrowArgumentNullException()
    {
        var name = new Criteria("Name");

        Assert.Throws<ArgumentNullException>(() => name.Contains(null!));
        Assert.Throws<ArgumentNullException>(() => name.StartsWith(null!));
        Assert.Throws<ArgumentNullException>(() => name.EndsWith(null!));
        Assert.Throws<ArgumentNullException>(() => name.NotContains(null!));
    }

    // Visitor / validator

    [Fact]
    public void VisitorRebuild_PreservesLikeEscapeChar()
    {
        var rewritten = new RenameNameVisitor().Rewrite(new Criteria("Name").Contains("a%b"));

        var binary = Assert.IsType<BinaryCriteria>(rewritten);
        Assert.Equal(CriteriaOperator.Like, binary.Operator);
        Assert.Equal('!', binary.LikeEscapeChar);
        Assert.Equal("Renamed", Assert.IsType<Criteria>(binary.LeftOperand).Expression);
        Assert.Equal("%a!%b%", Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void SafeCriteriaValidator_AcceptsEscapedLike()
    {
        // Note: upper: true wraps operands in UpperFunctionCriteria, which the
        // validator rejects like any function call — same as Like(x, upper: true) today.
        new SafeCriteriaValidator().Validate(new Criteria("Name").Contains("a%b"));
        new SafeCriteriaValidator().Validate(new Criteria("Name").NotContains("a[b"));
    }

    // System.Text.Json round-trip

    [Fact]
    public void Stj_Write_EscapedLike_WritesFourElementArray()
    {
        var json = JsonSerializer.Serialize<BaseCriteria>(
            new Criteria("Name").Contains("a%b"), GetStjOptions());

        Assert.Equal("[[\"Name\"],\"like\",\"%a!%b%\",\"!\"]", json);
    }

    [Fact]
    public void Stj_RoundTrip_PreservesEscape()
    {
        var options = GetStjOptions();
        var criteria = new Criteria("Name").LikeEscaped(
            "%" + Criteria.EscapeLikeWildcards("a[b", '/') + "%", escape: '/');

        var json = JsonSerializer.Serialize<BaseCriteria>(criteria, options);
        var result = Assert.IsType<BinaryCriteria>(
            JsonSerializer.Deserialize<BaseCriteria>(json, options));

        Assert.Equal(CriteriaOperator.Like, result.Operator);
        Assert.Equal('/', result.LikeEscapeChar);
        Assert.Equal(criteria.ToStringIgnoreParams(), result.ToStringIgnoreParams());
    }

    [Fact]
    public void Stj_Read_NonLikeOperator_WithEscape_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"=\",\"x\",\"!\"]", GetStjOptions()));
    }

    [Fact]
    public void Stj_Read_MultiCharEscape_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"like\",\"x\",\"!!\"]", GetStjOptions()));
    }

    [Fact]
    public void Stj_Read_WildcardEscapeChar_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<BaseCriteria>("[\"A\",\"like\",\"x\",\"%\"]", GetStjOptions()));
    }

    // Newtonsoft.Json round-trip

    private static Newtonsoft.Json.JsonSerializerSettings GetNewtonsoftSettings()
    {
        return new Newtonsoft.Json.JsonSerializerSettings();
    }

    [Fact]
    public void Newtonsoft_Write_EscapedLike_WritesFourElementArray()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(
            new Criteria("Name").Contains("a%b"), GetNewtonsoftSettings());

        Assert.Equal("[[\"Name\"],\"like\",\"%a!%b%\",\"!\"]", json);
    }

    [Fact]
    public void Newtonsoft_RoundTrip_PreservesEscape()
    {
        var settings = GetNewtonsoftSettings();
        var criteria = new Criteria("Name").StartsWith("a_b");

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(criteria, settings);
        var result = Assert.IsType<BinaryCriteria>(
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>(json, settings));

        Assert.Equal(CriteriaOperator.Like, result.Operator);
        Assert.Equal('!', result.LikeEscapeChar);
        Assert.Equal(criteria.ToStringIgnoreParams(), result.ToStringIgnoreParams());
    }

    [Fact]
    public void Newtonsoft_Read_NonLikeOperator_WithEscape_ThrowsJsonSerializationException()
    {
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>(
                "[\"A\",\"=\",\"x\",\"!\"]", GetNewtonsoftSettings()));
    }

    [Fact]
    public void Newtonsoft_Read_MultiCharEscape_ThrowsJsonSerializationException()
    {
        Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() =>
            Newtonsoft.Json.JsonConvert.DeserializeObject<BaseCriteria>(
                "[\"A\",\"like\",\"x\",\"!!\"]", GetNewtonsoftSettings()));
    }
}
