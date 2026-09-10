namespace Serenity.Data;

public class SqlTests
{
    // Sum

    [Fact]
    public void Sum_FieldName_ReturnsSumExpression()
    {
        Assert.Equal("SUM(Amount)", Sql.Sum("Amount"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Sum_StringField_NullOrEmpty_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Sum(field));
    }

    [Fact]
    public void Sum_IField_UsesExpression()
    {
        var field = IdNameRow.Fields.Name;

        Assert.Equal("SUM(" + field.Expression + ")", Sql.Sum(field));
    }

    [Fact]
    public void Sum_IField_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Sum((IField)null!));
    }

    [Fact]
    public void Sum_JoinNumber_WrapsT7()
    {
        Assert.Equal("SUM(T7.Name)", Sql.Sum(7, "Name"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Sum_JoinNumber_NullOrEmptyField_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Sum(7, field));
    }

    // Count

    [Fact]
    public void Count_FieldName_ReturnsCountExpression()
    {
        Assert.Equal("COUNT(Name)", Sql.Count("Name"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Count_StringField_NullOrEmpty_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Count(field));
    }

    [Fact]
    public void Count_IField_UsesExpression()
    {
        var field = IdNameRow.Fields.Name;

        Assert.Equal("COUNT(" + field.Expression + ")", Sql.Count(field));
    }

    [Fact]
    public void Count_IField_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Count((IField)null!));
    }

    [Fact]
    public void Count_JoinNumber_WrapsT7()
    {
        Assert.Equal("COUNT(T7.Name)", Sql.Count(7, "Name"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Count_JoinNumber_NullOrEmptyField_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Count(7, field));
    }

    [Fact]
    public void Count_NoArgs_ReturnsCountStar()
    {
        Assert.Equal("COUNT(*)", Sql.Count());
    }

    // Coalesce

    [Fact]
    public void Coalesce_MultipleStrings_Joins()
    {
        Assert.Equal("COALESCE(A, B, C)", Sql.Coalesce("A", "B", "C"));
    }

    [Fact]
    public void Coalesce_MultipleStrings_Single_ContainsValue()
    {
        Assert.Equal("COALESCE(A)", Sql.Coalesce("A"));
    }

    [Fact]
    public void Coalesce_Strings_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Coalesce((string[])null!));
    }

    [Fact]
    public void Coalesce_Strings_Empty_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Coalesce(new string[0]));
    }

    [Fact]
    public void Coalesce_Query_WithValues_AddsParams()
    {
        var query = new SqlQuery();

        Assert.Equal("COALESCE(@p1, @p2)", query.Coalesce(1, 2));
        Assert.Equal(2, query.Params!.Count);
    }

    [Fact]
    public void Coalesce_Query_NullOrEmpty_ThrowsArgumentNullException()
    {
        var query = new SqlQuery();

        Assert.Throws<ArgumentNullException>(() => query.Coalesce(null!));
        Assert.Throws<ArgumentNullException>(() => query.Coalesce([]));
    }

    [Fact]
    public void Coalesce_Query_WithCriteriaCriteria()
    {
        var query = new SqlQuery();
        var criteria = new Criteria("C");

        var result = query.Coalesce(criteria, criteria);

        Assert.Equal("COALESCE(C, C)", result);
    }

    [Fact]
    public void Coalesce_Query_WithSubQuery()
    {
        var query = new SqlQuery();
        var sub = new SqlQuery().Select("1");

        var result = query.Coalesce(sub);

        Assert.Contains("SELECT", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Coalesce_Query_WithField_UsesExpression()
    {
        var query = new SqlQuery();
        var field = IdNameRow.Fields.Name;

        var result = query.Coalesce(field, field);

        Assert.Contains(field.Expression, result, StringComparison.Ordinal);
    }

    // Min

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Min_StringField_NullOrEmpty_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Min(field));
    }

    [Fact]
    public void Min_StringField_ReturnsMinExpression()
    {
        Assert.Equal("MIN(A)", Sql.Min("A"));
    }

    [Fact]
    public void Min_IField_UsesExpression()
    {
        var field = IdNameRow.Fields.Name;

        Assert.Equal("MIN(" + field.Expression + ")", Sql.Min(field));
    }

    [Fact]
    public void Min_IField_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Min((IField)null!));
    }

    [Fact]
    public void Min_JoinNumber_WrapsT3()
    {
        Assert.Equal("MIN(T3.Name)", Sql.Min(3, "Name"));
    }

    // Max

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Max_StringField_NullOrEmpty_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Max(field));
    }

    [Fact]
    public void Max_StringField_ReturnsMaxExpression()
    {
        Assert.Equal("MAX(A)", Sql.Max("A"));
    }

    [Fact]
    public void Max_IField_UsesExpression()
    {
        var field = IdNameRow.Fields.Name;

        Assert.Equal("MAX(" + field.Expression + ")", Sql.Max(field));
    }

    [Fact]
    public void Max_IField_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Max((IField)null!));
    }

    [Fact]
    public void Max_JoinNumber_WrapsT3()
    {
        Assert.Equal("MAX(T3.Name)", Sql.Max(3, "Name"));
    }

    // Avg

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Avg_StringField_NullOrEmpty_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Avg(field));
    }

    [Fact]
    public void Avg_StringField_ReturnsAvgExpression()
    {
        Assert.Equal("AVG(A)", Sql.Avg("A"));
    }

    [Fact]
    public void Avg_IField_UsesExpression()
    {
        var field = IdNameRow.Fields.Name;

        Assert.Equal("AVG(" + field.Expression + ")", Sql.Avg(field));
    }

    [Fact]
    public void Avg_IField_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Avg((IField)null!));
    }

    [Fact]
    public void Avg_JoinNumber_WrapsT5()
    {
        Assert.Equal("AVG(T5.Name)", Sql.Avg(5, "Name"));
    }

    // Convert / SubString

    [Fact]
    public void Convert_ReturnsConvertExpression()
    {
        Assert.Equal(" Convert(DATE,Field) ", Sql.Convert("DATE", "Field"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Convert_NullOrEmptyType_ThrowsArgumentNullException(string type)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Convert(type, "Field"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Convert_NullOrEmptyField_ThrowsArgumentNullException(string field)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.Convert("DATE", field));
    }

    [Fact]
    public void SubString_ReturnsSubstringExpression()
    {
        Assert.Equal(" substring(A,1,2) ", Sql.SubString("A", 1, 2));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SubString_NullOrEmptyExpression_ThrowsArgumentNullException(string expression)
    {
        Assert.Throws<ArgumentNullException>(() => Sql.SubString(expression, 0, 1));
    }

    // Case (string-based)

    [Fact]
    public void Case_WithConditionAndPairs_BuildsStatement()
    {
        var result = Sql.Case("Field1", ["1", "'ResultA'", "2", "'ResultB'"], "'Else'");

        Assert.Equal("CASE Field1 WHEN 1 THEN 'ResultA' WHEN 2 THEN 'ResultB' ELSE 'Else' END", result);
    }

    [Fact]
    public void Case_WithoutElse_OmitsElse()
    {
        var result = Sql.Case("Field1", ["1", "'ResultA'"], null);

        Assert.Equal("CASE Field1 WHEN 1 THEN 'ResultA' END", result);
    }

    [Fact]
    public void Case_EmptyPairs_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Sql.Case("Field1", [], null));
    }

    [Fact]
    public void Case_OddNumberOfPairs_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Sql.Case("Field1", ["1"], null));
    }

    // Case (builder)

    [Fact]
    public void Case_Query_Builder_BuildsStatementWithParams()
    {
        var query = new SqlQuery();

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.WhenThen(new Criteria("B") == 2, 10);
            cb.Else("x");
        });

        Assert.Equal("CASE  WHEN (A = @p1) THEN @p2 WHEN (B = @p3) THEN @p4 ELSE @p5 END", result);
    }

    [Fact]
    public void Case_Query_Builder_NoWhenThen_ThrowsInvalidOperationException()
    {
        var query = new SqlQuery();

        Assert.Throws<InvalidOperationException>(() =>
            query.Case(cb => { }));
    }

    [Fact]
    public void Case_Query_Builder_MismatchedWhenThen_ThrowsInvalidOperationException()
    {
        var query = new SqlQuery();

        Assert.Throws<InvalidOperationException>(() =>
            query.Case(cb =>
            {
                cb.When(new Criteria("A") == 1);
            }));
    }

    [Fact]
    public void Case_Query_Builder_DoubleElse_ThrowsInvalidOperationException()
    {
        var query = new SqlQuery();

        Assert.Throws<InvalidOperationException>(() =>
            query.Case(cb =>
            {
                cb.WhenThen(new Criteria("A") == 1, "x");
                cb.Else(1);
                cb.Else(2);
            }));
    }

    [Fact]
    public void Case_Query_Builder_ElseSupportsICriteria()
    {
        var query = new SqlQuery();

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.Else(new Criteria("B"));
        });

        Assert.Contains(" ELSE B END", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Case_Query_Builder_ElseSupportsQuery()
    {
        var query = new SqlQuery();
        var sub = new SqlQuery().Select("1");

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.Else(sub);
        });

        Assert.Contains(" ELSE SELECT", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Case_Query_Builder_ElseSupportsField()
    {
        var query = new SqlQuery();

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.Else(IdNameRow.Fields.Name);
        });

        Assert.Contains(" ELSE " + IdNameRow.Fields.Name.Expression, result, StringComparison.Ordinal);
    }

    [Fact]
    public void Case_Query_Builder_NullElse_AddsDbNullParam()
    {
        var query = new SqlQuery();

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.Else(null!);
        });

        Assert.StartsWith("CASE", result, StringComparison.Ordinal);
        Assert.Contains(" ELSE ", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Case_Query_Builder_SupportsThenVariants()
    {
        var query = new SqlQuery();
        var field = IdNameRow.Fields.Name;
        var sub = new SqlQuery().Select("1");

        var result = query.Case(cb =>
        {
            cb.When(new Criteria("A") == 1);
            cb.Then("direct");
            cb.When(new Criteria("A") == 2);
            cb.Then(field);
            cb.When(new Criteria("A") == 3);
            cb.Then(sub);
        });

        Assert.StartsWith("CASE", result, StringComparison.Ordinal);
        Assert.EndsWith("END", result, StringComparison.Ordinal);
        Assert.Contains(field.Expression, result, StringComparison.Ordinal);
        Assert.Contains("SELECT", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Case_Query_Builder_ElseDBNull_ConvertsToParam()
    {
        var query = new SqlQuery();

        var result = query.Case(cb =>
        {
            cb.WhenThen(new Criteria("A") == 1, "'R1'");
            cb.Else(null!);
        });

        // else value null is stored as DBNull, rendered as a param
        Assert.Contains("@", result, StringComparison.Ordinal);
    }

    // CaseBuilder.WhenThen returns builder for chaining

    [Fact]
    public void CaseBuilder_WhenThen_ReturnsSameInstance()
    {
        var cb = new Sql.CaseBuilder();

        Assert.Same(cb, cb.WhenThen(new Criteria("A") == 1, "x"));
        Assert.Same(cb, cb.When(new Criteria("B") == 2));
        Assert.Same(cb, cb.Then(1));
        Assert.Same(cb, cb.Else(4));
    }
}
