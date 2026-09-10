namespace Serenity.Data;

public class ConstantAndParamCriteriaTests
{
    [Fact]
    public void ConstantCriteria_IntValue()
    {
        Assert.Equal("5", new ConstantCriteria(5).ToString());
    }

    [Fact]
    public void ConstantCriteria_LongValue()
    {
        Assert.Equal("5", new ConstantCriteria(5L).ToString());
    }

    [Fact]
    public void ConstantCriteria_IntArray()
    {
        Assert.Equal("1,2,3", new ConstantCriteria([1, 2, 3]).ToString());
    }

    [Fact]
    public void ConstantCriteria_LongArray()
    {
        Assert.Equal("1,2", new ConstantCriteria(new long[] { 1, 2 }).ToString());
    }

    [Fact]
    public void ConstantCriteria_StringValue_IsQuoted()
    {
        // Default dialect (SqlServer2012) quotes unicode strings with the N'...' prefix.
        Assert.Equal("N'x'", new ConstantCriteria("x").ToString());
    }

    [Fact]
    public void ConstantCriteria_StringValue_WithQuote_IsEscaped()
    {
        Assert.Equal("N'x''y'", new ConstantCriteria("x'y").ToString());
    }

    [Fact]
    public void ConstantCriteria_StringArray()
    {
        Assert.Equal("N'a',N'b'", new ConstantCriteria(["a", "b"]).ToString());
    }

    [Fact]
    public void ConstantCriteria_StringValue_WithDialect()
    {
        Assert.Equal("N'x'", new ConstantCriteria("x", SqlServer2012Dialect.Instance).ToString());
        Assert.Equal("N'x''y'", new ConstantCriteria("x'y", SqlServer2012Dialect.Instance).ToString());
    }

    [Fact]
    public void ConstantCriteria_ToStringWithoutQuery_Works()
    {
        // Constant criteria never produce parameters, so ToString() works without a query.
        Assert.Equal("5", new ConstantCriteria(5).ToString());
        Assert.Equal("N'x'", new ConstantCriteria("x").ToString());
        Assert.Equal("1,2,3", new ConstantCriteria([1, 2, 3]).ToString());
    }

    [Fact]
    public void ParamCriteria_SetsName()
    {
        var criteria = new ParamCriteria("@p1");

        Assert.Equal("@p1", criteria.Name);
        Assert.Equal("@p1", criteria.ToString());
    }

    [Fact]
    public void ParamCriteria_NullOrEmptyName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ParamCriteria(null));
        Assert.Throws<ArgumentNullException>(() => new ParamCriteria(""));
    }

    [Fact]
    public void ParamCriteria_NameWithoutAtSign_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ParamCriteria("p1"));
    }

    [Fact]
    public void UpperFunctionCriteria_ToString()
    {
        Assert.Equal("UPPER(x)", new UpperFunctionCriteria(new Criteria("x")).ToString());
    }

    [Fact]
    public void UpperFunctionCriteria_GetFunctionName()
    {
        var func = new UpperFunctionCriteria(new Criteria("x"));

        Assert.Equal("UPPER", func.GetFunctionName(SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void UpperFunctionCriteria_Arguments()
    {
        var arg = new Criteria("x");
        var func = new UpperFunctionCriteria(arg);

        var argument = Assert.Single(func.Arguments);
        Assert.Same(arg, argument);
    }

    [Fact]
    public void FunctionCallCriteria_CustomFunction_WithArguments()
    {
        var func = new TestFunc(new Criteria("a"), new Criteria("b"));

        Assert.Equal("MYFUNC(a, b)", func.ToString());
    }

    [Fact]
    public void FunctionCallCriteria_CustomFunction_EmptyArguments()
    {
        Assert.Equal("MYFUNC()", new TestFunc().ToString());
    }

    [Fact]
    public void FunctionCallCriteria_AppendHooks_AreCalled()
    {
        var func = new HookedFunc(new Criteria("x"));

        Assert.Equal("HOOKED(x)", func.ToString());
        Assert.True(func.FunctionNameAppended);
        Assert.True(func.OpenParenAppended);
        Assert.True(func.ArgumentsAppended);
        Assert.True(func.CloseParenAppended);
    }

    private class TestFunc(params BaseCriteria[] args) : FunctionCallCriteria(args)
    {
        public override string GetFunctionName(ISqlDialect dialect)
        {
            return "MYFUNC";
        }
    }

    private class HookedFunc(params BaseCriteria[] args) : FunctionCallCriteria(args)
    {
        public bool FunctionNameAppended { get; private set; }
        public bool OpenParenAppended { get; private set; }
        public bool ArgumentsAppended { get; private set; }
        public bool CloseParenAppended { get; private set; }

        public override string GetFunctionName(ISqlDialect dialect)
        {
            return "HOOKED";
        }

        protected override void AppendFunctionName(StringBuilder sb, IQueryWithParams query)
        {
            FunctionNameAppended = true;
            base.AppendFunctionName(sb, query);
        }

        protected override void AppendOpenParenthesis(StringBuilder sb, IQueryWithParams query)
        {
            OpenParenAppended = true;
            base.AppendOpenParenthesis(sb, query);
        }

        protected override void AppendArguments(StringBuilder sb, IQueryWithParams query)
        {
            ArgumentsAppended = true;
            base.AppendArguments(sb, query);
        }

        protected override void AppendCloseParenthesis(StringBuilder sb, IQueryWithParams query)
        {
            CloseParenAppended = true;
            base.AppendCloseParenthesis(sb, query);
        }
    }
}
