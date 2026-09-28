namespace Serenity.Data;

public class JoinAliasLocatorTests
{
    [Fact]
    public void Locate_Null_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => JoinAliasLocator.Locate(null));
    }

    [Fact]
    public void Locate_Without_Aliases_Returns_Null()
    {
        Assert.Null(JoinAliasLocator.Locate("Field"));
        Assert.Null(JoinAliasLocator.Locate("select * from t"));
    }

    [Fact]
    public void Locate_Finds_Single_Alias()
    {
        var aliases = JoinAliasLocator.Locate("T0.Field");

        Assert.NotNull(aliases);
        Assert.Equal("T0", Assert.Single(aliases));
    }

    [Fact]
    public void Locate_Finds_Multiple_Aliases()
    {
        var aliases = JoinAliasLocator.Locate("T0.a = T1.b");

        Assert.NotNull(aliases);
        Assert.Contains("T0", aliases);
        Assert.Contains("T1", aliases);
        Assert.Equal(2, aliases.Count);
    }

    [Fact]
    public void Locate_Is_Case_Insensitive()
    {
        var aliases = JoinAliasLocator.Locate("T0.a = t0.b");

        Assert.NotNull(aliases);
        Assert.Equal("T0", Assert.Single(aliases));
    }

    [Fact]
    public void Locate_Finds_Each_Identifier_Before_A_Dot()
    {
        var aliases = JoinAliasLocator.Locate("a.b.c");

        Assert.NotNull(aliases);
        Assert.Contains("a", aliases);
        Assert.Contains("b", aliases);
        Assert.Equal(2, aliases.Count);
    }

    [Fact]
    public void Locate_Ignores_Aliases_In_Quotes()
    {
        Assert.Null(JoinAliasLocator.Locate("'T0.x'"));
    }

    [Fact]
    public void Locate_Ignores_Digit_Only_Identifiers()
    {
        Assert.Null(JoinAliasLocator.Locate("123.x"));
    }

    [Fact]
    public void Locate_Finds_Underscore_Identifiers()
    {
        var aliases = JoinAliasLocator.Locate("_x.y");

        Assert.NotNull(aliases);
        Assert.Equal("_x", Assert.Single(aliases));
    }

    [Fact]
    public void LocateOptimized_With_Single_Alias_Returns_SingleAlias()
    {
        var aliases = JoinAliasLocator.LocateOptimized("T0.x", out var singleAlias);

        Assert.Null(aliases);
        Assert.Equal("T0", singleAlias);
    }

    [Fact]
    public void LocateOptimized_Without_Aliases_Returns_Nulls()
    {
        var aliases = JoinAliasLocator.LocateOptimized("Field", out var singleAlias);

        Assert.Null(aliases);
        Assert.Null(singleAlias);
    }

    [Fact]
    public void LocateOptimized_With_Multiple_Aliases_Returns_Set()
    {
        var aliases = JoinAliasLocator.LocateOptimized("T0.a T1.b", out var singleAlias);

        Assert.NotNull(aliases);
        Assert.Contains("T0", aliases);
        Assert.Contains("T1", aliases);
        Assert.Null(singleAlias);
    }

    [Fact]
    public void LocateOptimized_With_Same_Alias_Twice_Returns_SingleAlias()
    {
        var aliases = JoinAliasLocator.LocateOptimized("T0.a T0.b", out var singleAlias);

        Assert.Null(aliases);
        Assert.Equal("T0", singleAlias);
    }

    [Fact]
    public void EnumerateAliases_Calls_Handler_For_Each_Alias()
    {
        var list = new List<string>();

        var result = JoinAliasLocator.EnumerateAliases("T0.a = T1.b", list.Add);

        Assert.True(result);
        Assert.Equal(["T0", "T1"], list);
    }

    [Fact]
    public void ReplaceAliases_Replaces_Alias_Part()
    {
        Assert.Equal("j.Field",
            JoinAliasLocator.ReplaceAliases("T0.Field", s => s == "T0" ? "j" : s));
    }

    [Fact]
    public void ReplaceAliases_Rewrites_Criteria_Tree_And_Preserves_Parameters()
    {
        var parameter = new ParamCriteria("@id");
        var criteria = new BinaryCriteria(
            new BinaryCriteria(new Criteria("T0.Id"), CriteriaOperator.EQ, new Criteria("T0.ParentId")),
            CriteriaOperator.AND,
            new BinaryCriteria(new Criteria("T1.Id"), CriteriaOperator.EQ, parameter));

        var result = Assert.IsType<BinaryCriteria>(JoinAliasLocator.ReplaceAliases(criteria, alias =>
            alias == "T0" ? "j0" : alias == "T1" ? "j1" : alias));

        var left = Assert.IsType<BinaryCriteria>(result.LeftOperand);
        Assert.Equal("j0.Id", Assert.IsType<Criteria>(left.LeftOperand).Expression);
        Assert.Equal("j0.ParentId", Assert.IsType<Criteria>(left.RightOperand).Expression);

        var right = Assert.IsType<BinaryCriteria>(result.RightOperand);
        Assert.Equal("j1.Id", Assert.IsType<Criteria>(right.LeftOperand).Expression);
        Assert.Same(parameter, right.RightOperand);
    }

    [Fact]
    public void ReplaceAliases_Rewrites_FunctionCall_Arguments_Without_Mutating_Original()
    {
        var originalArgument = new Criteria("T0.Name");
        var original = new UpperFunctionCriteria(originalArgument);

        var result = Assert.IsType<UpperFunctionCriteria>(JoinAliasLocator.ReplaceAliases(original,
            alias => alias == "T0" ? "T1" : alias));

        Assert.NotSame(original, result);
        Assert.Equal("T1.Name", Assert.IsType<Criteria>(result.Arguments[0]).Expression);
        Assert.Same(originalArgument, original.Arguments[0]);
    }

    [Fact]
    public void ReplaceAliases_Uses_FunctionCall_Clone_Hook()
    {
        var originalArgument = new Criteria("T0.Name");
        var original = new CustomFunctionCriteria("custom state", originalArgument);

        var result = Assert.IsType<CustomFunctionCriteria>(JoinAliasLocator.ReplaceAliases(original,
            alias => alias == "T0" ? "T1" : alias));

        Assert.NotSame(original, result);
        Assert.Equal("custom state", result.State);
        Assert.Equal("T1.Name", Assert.IsType<Criteria>(result.Arguments[0]).Expression);
        Assert.Same(originalArgument, original.Arguments[0]);
    }

    [Fact]
    public void ReplaceAliases_Keeps_Expression_When_Replacement_Is_Same()
    {
        Assert.Equal("T0.Field",
            JoinAliasLocator.ReplaceAliases("T0.Field", s => s));
    }

    [Fact]
    public void ReplaceAliases_Replaces_All_Aliases()
    {
        Assert.Equal("j1.a = j2.b",
            JoinAliasLocator.ReplaceAliases("T0.a = T1.b",
                s => s == "T0" ? "j1" : s == "T1" ? "j2" : s));
    }

    [Fact]
    public void ReplaceAliases_Ignores_Aliases_In_Quotes()
    {
        Assert.Equal("'T0.x'",
            JoinAliasLocator.ReplaceAliases("'T0.x'", s => "j"));
    }

    [Fact]
    public void ReplaceAliases_Keeps_Identifiers_Without_Dot()
    {
        Assert.Equal("Field",
            JoinAliasLocator.ReplaceAliases("Field", s => "j"));
    }

    [Fact]
    public void ReplaceAliases_Replaces_Each_Identifier_Before_A_Dot()
    {
        Assert.Equal("X.Y.c",
            JoinAliasLocator.ReplaceAliases("a.b.c",
                s => s == "a" ? "X" : s == "b" ? "Y" : s));
    }

    private sealed class CustomFunctionCriteria(string state, params BaseCriteria[] arguments)
        : FunctionCallCriteria(arguments)
    {
        public string State { get; } = state;

        public override string GetFunctionName(ISqlDialect dialect) => "CUSTOM";

        protected internal override FunctionCallCriteria CloneWithArguments(BaseCriteria[] arguments)
        {
            return new CustomFunctionCriteria(State, arguments);
        }
    }
}
