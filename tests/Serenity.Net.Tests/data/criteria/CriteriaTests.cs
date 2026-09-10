namespace Serenity.Data;

public class CriteriaTests
{
    [Fact]
    public void Constructor_WithExpression_SetsExpression()
    {
        var criteria = new Criteria("Name");

        Assert.Equal("Name", criteria.Expression);
        Assert.Equal("Name", criteria.ToString());
        Assert.False(criteria.IsEmpty);
    }

    [Fact]
    public void Constructor_WithEmptyExpression_IsEmpty()
    {
        var criteria = new Criteria("");

        Assert.True(criteria.IsEmpty);
        Assert.Equal("", criteria.ToString());
    }

    [Fact]
    public void Constructor_WithField_UsesFieldExpression()
    {
        var criteria = new Criteria(AllFieldsRow.Fields.AString);

        Assert.Equal("T0.[AString]", criteria.Expression);
        Assert.Equal("T0.[AString]", criteria.ToString());
    }

    [Fact]
    public void Constructor_WithNullField_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Criteria((IField)null));

        Assert.Equal("field", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithAliasAndField_BracketsFieldName()
    {
        var criteria = new Criteria("T0", "Name");

        Assert.Equal("T0.[Name]", criteria.Expression);
        Assert.Equal("T0.[Name]", criteria.ToString());
    }

    [Fact]
    public void Constructor_WithAliasAndField_NullOrEmptyArguments_ThrowArgumentNullException()
    {
        Assert.Equal("alias", Assert.Throws<ArgumentNullException>(
            () => new Criteria((string)null, "Name")).ParamName);
        Assert.Equal("alias", Assert.Throws<ArgumentNullException>(
            () => new Criteria("", "Name")).ParamName);
        Assert.Equal("field", Assert.Throws<ArgumentNullException>(
            () => new Criteria("T0", (string)null)).ParamName);
        Assert.Equal("field", Assert.Throws<ArgumentNullException>(
            () => new Criteria("T0", "")).ParamName);
    }

    [Fact]
    public void Constructor_WithJoinStringAndField_UsesFieldName()
    {
        Assert.Equal("T0.[AString]", new Criteria("T0", AllFieldsRow.Fields.AString).ToString());
    }

    [Fact]
    public void Constructor_WithJoinNumber_PrefixesTableAlias()
    {
        Assert.Equal("T0.[Name]", new Criteria(0, "Name").ToString());
        Assert.Equal("T3.[Name]", new Criteria(3, "Name").ToString());
    }

    [Fact]
    public void Constructor_WithNegativeJoinNumber_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Criteria(-1, "Name"));
    }

    [Fact]
    public void Constructor_WithJoinNumberAndNullField_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Criteria(0, (IField)null));
        Assert.Equal("field", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithAliasInstanceAndField()
    {
        Assert.Equal("x.[AString]", new Criteria(new Alias("x"), AllFieldsRow.Fields.AString).ToString());
    }

    [Fact]
    public void Constructor_WithNullAliasInstance_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new Criteria((IAlias)null, AllFieldsRow.Fields.AString));
        Assert.Equal("alias", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithAliasInstanceAndNullField_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new Criteria(new Alias("x"), (IField)null));
        Assert.Equal("field", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullAliasInstanceAndStringField_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new Criteria((IAlias)null, "Name"));
        Assert.Equal("alias", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithJoinStringAndNullField_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new Criteria("x", (IField)null));
        Assert.Equal("field", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullQuery_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new Criteria((ISqlQuery)null));
        Assert.Equal("query", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithAliasInstanceAndStringField()
    {
        Assert.Equal("x.[Name]", new Criteria(new Alias("x"), "Name").ToString());
    }

    [Fact]
    public void Constructor_WithAliasInstanceAndNullStringField_ThrowsArgumentNullException()
    {
        Assert.Equal("field", Assert.Throws<ArgumentNullException>(
            () => new Criteria(new Alias("x"), (string)null)).ParamName);
    }

    [Fact]
    public void Constructor_WithJoinNumberAndField()
    {
        Assert.Equal("T0.[AString]", new Criteria(0, AllFieldsRow.Fields.AString).ToString());
    }

    [Fact]
    public void Constructor_WithJoinStringAndIField()
    {
        Assert.Equal("j.[AString]", new Criteria("j", AllFieldsRow.Fields.AString).ToString());
    }

    [Fact]
    public void Constructor_WithFieldAndCustomExpression_UsesExpression()
    {
        var criteria = new Criteria(AllFieldsRow.Fields.AString, "custom");

        Assert.Equal("custom", criteria.Expression);
        Assert.Equal("custom", criteria.ToString());
    }

    [Fact]
    public void Constructor_WithFieldAndExpression_NullField_ThrowsArgumentNullException()
    {
        Assert.Equal("field", Assert.Throws<ArgumentNullException>(
            () => new Criteria((IField)null, "custom")).ParamName);
    }

    [Fact]
    public void Constructor_WithQuery_UsesQueryText()
    {
        var query = new SqlQuery().From("Table").Select("x");

        var criteria = new Criteria(query);

        // SqlQuery.ToString() is multi-line; compare against it directly.
        Assert.Equal(query.ToString(), criteria.Expression);
        Assert.Equal(query.ToString(), criteria.ToString());
    }

    [Fact]
    public void Empty_IsEmptyCriteria()
    {
        Assert.True(Criteria.Empty.IsEmpty);
        Assert.Equal("", Criteria.Empty.ToString());
    }

    [Fact]
    public void False_And_True_HaveExpectedText()
    {
        Assert.Equal("0=1", Criteria.False.ToString());
        Assert.Equal("1=1", Criteria.True.ToString());
    }

    [Fact]
    public void Bracket_WrapsFieldNameInBrackets()
    {
        var criteria = Criteria.Bracket("Name");

        Assert.Equal("[Name]", criteria.Expression);
        Assert.Equal("[Name]", criteria.ToString());
    }

    [Fact]
    public void Bracket_NullOrEmpty_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Criteria.Bracket(null));
        Assert.Throws<ArgumentNullException>(() => Criteria.Bracket(""));
    }

    [Fact]
    public void Exists_WithQuery_WrapsQueryInExists()
    {
        var query = new SqlQuery().From("Table").Select("x");

        var criteria = Criteria.Exists(query);

        // SqlQuery.ToString() is multi-line; compare against it directly.
        Assert.Equal("EXISTS (" + query.ToString() + ")", criteria.ToString());
    }

    [Fact]
    public void Exists_WithExpression_WrapsExpressionInExists()
    {
        Assert.Equal("EXISTS (expr)", Criteria.Exists("expr").ToString());
    }
}
