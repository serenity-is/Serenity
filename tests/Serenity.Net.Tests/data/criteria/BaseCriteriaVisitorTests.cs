namespace Serenity.Data;

public class BaseCriteriaVisitorTests
{
    private class RecordingVisitor : BaseCriteriaVisitor
    {
        public List<string> Visited { get; } = [];

        public BaseCriteria? VisitPublic(BaseCriteria? criteria)
        {
            return Visit(criteria);
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            Visited.Add("Criteria:" + criteria.Expression);
            return base.VisitCriteria(criteria);
        }

        protected override BaseCriteria VisitBinary(BinaryCriteria criteria)
        {
            Visited.Add("Binary");
            return base.VisitBinary(criteria);
        }

        protected override BaseCriteria VisitUnary(UnaryCriteria criteria)
        {
            Visited.Add("Unary");
            return base.VisitUnary(criteria);
        }

        protected override BaseCriteria VisitValue(ValueCriteria criteria)
        {
            Visited.Add("Value");
            return base.VisitValue(criteria);
        }

        protected override BaseCriteria VisitParam(ParamCriteria criteria)
        {
            Visited.Add("Param");
            return base.VisitParam(criteria);
        }

        protected override BaseCriteria VisitFunctionCall(FunctionCallCriteria criteria)
        {
            Visited.Add("FunctionCall");
            return base.VisitFunctionCall(criteria);
        }
    }

    private class ValueReplacingVisitor : BaseCriteriaVisitor
    {
        public BaseCriteria? VisitPublic(BaseCriteria? criteria)
        {
            return Visit(criteria);
        }

        protected override BaseCriteria VisitValue(ValueCriteria criteria)
        {
            return new ConstantCriteria(5);
        }
    }

    private class CriteriaReplacingVisitor : BaseCriteriaVisitor
    {
        public BaseCriteria? VisitPublic(BaseCriteria? criteria)
        {
            return Visit(criteria);
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            return new Criteria("Replaced");
        }
    }

    private class UnknownCriteria : BaseCriteria
    {
    }

    [Fact]
    public void Visit_Criteria_DispatchesToVisitCriteria()
    {
        var visitor = new RecordingVisitor();
        var criteria = new Criteria("Name");

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["Criteria:Name"], visitor.Visited);
    }

    [Fact]
    public void Visit_Binary_VisitsBothOperands()
    {
        var visitor = new RecordingVisitor();
        var left = new Criteria("Name");
        var right = new Criteria("Other");
        var criteria = new BinaryCriteria(left, CriteriaOperator.AND, right);

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["Binary", "Criteria:Name", "Criteria:Other"], visitor.Visited);
    }

    [Fact]
    public void Visit_Unary_VisitsOperand()
    {
        var visitor = new RecordingVisitor();
        var operand = new Criteria("Name");
        var criteria = new UnaryCriteria(CriteriaOperator.Not, operand);

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["Unary", "Criteria:Name"], visitor.Visited);
    }

    [Fact]
    public void Visit_Value_DispatchesToVisitValue()
    {
        var visitor = new RecordingVisitor();
        var criteria = new ValueCriteria("x");

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["Value"], visitor.Visited);
    }

    [Fact]
    public void Visit_Param_DispatchesToVisitParam()
    {
        var visitor = new RecordingVisitor();
        var criteria = new ParamCriteria("@p1");

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["Param"], visitor.Visited);
    }

    [Fact]
    public void Visit_FunctionCall_DispatchesToVisitFunctionCall()
    {
        var visitor = new RecordingVisitor();
        var criteria = new UpperFunctionCriteria(new Criteria("Name"));

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        Assert.Equal(["FunctionCall"], visitor.Visited);
    }

    [Fact]
    public void Visit_Null_ReturnsNull()
    {
        var visitor = new RecordingVisitor();

        Assert.Null(visitor.VisitPublic(null));
        Assert.Empty(visitor.Visited);
    }

    [Fact]
    public void VisitBinary_RebuildsCriteriaWhenValueIsReplaced()
    {
        var visitor = new ValueReplacingVisitor();
        var left = new Criteria("Name");
        var original = new BinaryCriteria(left, CriteriaOperator.EQ, new ValueCriteria("x"));

        var result = visitor.VisitPublic(original);

        var rebuilt = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal(CriteriaOperator.EQ, rebuilt.Operator);
        Assert.Same(left, rebuilt.LeftOperand);
        Assert.Equal("5", Assert.IsType<ConstantCriteria>(rebuilt.RightOperand).Expression);

        // Original criteria tree is left unchanged.
        Assert.IsType<ValueCriteria>(original.RightOperand);
    }

    [Fact]
    public void VisitUnary_RebuildsCriteriaWhenOperandIsReplaced()
    {
        var visitor = new CriteriaReplacingVisitor();
        var original = new UnaryCriteria(CriteriaOperator.Not, new Criteria("Name"));

        var result = visitor.VisitPublic(original);

        var rebuilt = Assert.IsType<UnaryCriteria>(result);
        Assert.Equal(CriteriaOperator.Not, rebuilt.Operator);
        Assert.Equal("Replaced", Assert.IsType<Criteria>(rebuilt.Operand).Expression);

        // Original criteria tree is left unchanged.
        Assert.Equal("Name", Assert.IsType<Criteria>(original.Operand).Expression);
    }

    [Fact]
    public void Visit_UnknownCriteriaType_ThrowsInvalidProgramException()
    {
        var visitor = new RecordingVisitor();

        var exception = Assert.Throws<InvalidProgramException>(
            () => visitor.VisitPublic(new UnknownCriteria()));

        Assert.Equal("Unhandled criteria type: 'UnknownCriteria'", exception.Message);
        Assert.Empty(visitor.Visited);
    }

    [Fact]
    public void VisitFunctionCall_DoesNotDescendIntoArguments()
    {
        var visitor = new RecordingVisitor();
        var criteria = new UpperFunctionCriteria(new ValueCriteria("x"));

        var result = visitor.VisitPublic(criteria);

        Assert.Same(criteria, result);
        // Only the function call itself is visited; its arguments are not descended into.
        Assert.Equal(["FunctionCall"], visitor.Visited);
    }
}
