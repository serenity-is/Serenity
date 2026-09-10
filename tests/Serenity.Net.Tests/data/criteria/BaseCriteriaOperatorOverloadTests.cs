namespace Serenity.Data;

public class BaseCriteriaOperatorOverloadTests
{
    private enum TestEnum
    {
        A = 1
    }

    private static readonly DateTime testDate = new(2023, 5, 1, 12, 0, 0);
    private static readonly Guid testGuid = new("b1a9b1c7-2b39-4b48-b1ce-8a2a5a36ad80");

    private static void AssertBinary(BaseCriteria criteria, CriteriaOperator op, object? value)
    {
        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(op, binary.Operator);
        Assert.Equal(value, Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    // != overloads

    private static void AssertNotEqual(BaseCriteria result, object expected)
    {
        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal(CriteriaOperator.NE, binary.Operator);
        Assert.Equal(expected, Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
    }

    [Fact]
    public void NotEqual_Long() => AssertNotEqual(new Criteria("A") != 5L, 5L);

    [Fact]
    public void NotEqual_String() => AssertNotEqual(new Criteria("A") != "x", "x");

    [Fact]
    public void NotEqual_Double() => AssertNotEqual(new Criteria("A") != 5.5, 5.5);

    [Fact]
    public void NotEqual_Decimal() => AssertNotEqual(new Criteria("A") != 5.5m, 5.5m);

    [Fact]
    public void NotEqual_DateTime() => AssertNotEqual(new Criteria("A") != testDate, testDate);

    [Fact]
    public void NotEqual_Guid() => AssertNotEqual(new Criteria("A") != testGuid, testGuid);

    [Fact]
    public void NotEqual_Enum() => AssertNotEqual(new Criteria("A") != TestEnum.A, TestEnum.A);

    // > overloads

    [Fact]
    public void Greater_BaseCriteria() => AssertOp(new Criteria("A") > new Criteria("B"), CriteriaOperator.GT, "B");

    [Fact]
    public void Greater_Parameter() => AssertOpParam(new Criteria("A") > new Parameter("@p1"), CriteriaOperator.GT, "@p1");

    [Fact]
    public void Greater_Int() => AssertBinary(new Criteria("A") > 5, CriteriaOperator.GT, 5);

    [Fact]
    public void Greater_Long() => AssertBinary(new Criteria("A") > 5L, CriteriaOperator.GT, 5L);

    [Fact]
    public void Greater_String() => AssertBinary(new Criteria("A") > "x", CriteriaOperator.GT, "x");

    [Fact]
    public void Greater_Double() => AssertBinary(new Criteria("A") > 5.5, CriteriaOperator.GT, 5.5);

    [Fact]
    public void Greater_Decimal() => AssertBinary(new Criteria("A") > 5.5m, CriteriaOperator.GT, 5.5m);

    [Fact]
    public void Greater_DateTime() => AssertBinary(new Criteria("A") > testDate, CriteriaOperator.GT, testDate);

    [Fact]
    public void Greater_Guid() => AssertBinary(new Criteria("A") > testGuid, CriteriaOperator.GT, testGuid);

    [Fact]
    public void Greater_Enum() => AssertBinary(new Criteria("A") > TestEnum.A, CriteriaOperator.GT, TestEnum.A);

    // >= overloads

    [Fact]
    public void GreaterEqual_BaseCriteria() => AssertOp(new Criteria("A") >= new Criteria("B"), CriteriaOperator.GE, "B");

    [Fact]
    public void GreaterEqual_Parameter() => AssertOpParam(new Criteria("A") >= new Parameter("@p1"), CriteriaOperator.GE, "@p1");

    [Fact]
    public void GreaterEqual_Int() => AssertBinary(new Criteria("A") >= 5, CriteriaOperator.GE, 5);

    [Fact]
    public void GreaterEqual_Long() => AssertBinary(new Criteria("A") >= 5L, CriteriaOperator.GE, 5L);

    [Fact]
    public void GreaterEqual_String() => AssertBinary(new Criteria("A") >= "x", CriteriaOperator.GE, "x");

    [Fact]
    public void GreaterEqual_Double() => AssertBinary(new Criteria("A") >= 5.5, CriteriaOperator.GE, 5.5);

    [Fact]
    public void GreaterEqual_Decimal() => AssertBinary(new Criteria("A") >= 5.5m, CriteriaOperator.GE, 5.5m);

    [Fact]
    public void GreaterEqual_DateTime() => AssertBinary(new Criteria("A") >= testDate, CriteriaOperator.GE, testDate);

    [Fact]
    public void GreaterEqual_Guid() => AssertBinary(new Criteria("A") >= testGuid, CriteriaOperator.GE, testGuid);

    [Fact]
    public void GreaterEqual_Enum() => AssertBinary(new Criteria("A") >= TestEnum.A, CriteriaOperator.GE, TestEnum.A);

    // < overloads

    [Fact]
    public void Less_BaseCriteria() => AssertOp(new Criteria("A") < new Criteria("B"), CriteriaOperator.LT, "B");

    [Fact]
    public void Less_Parameter() => AssertOpParam(new Criteria("A") < new Parameter("@p1"), CriteriaOperator.LT, "@p1");

    [Fact]
    public void Less_Int() => AssertBinary(new Criteria("A") < 5, CriteriaOperator.LT, 5);

    [Fact]
    public void Less_Long() => AssertBinary(new Criteria("A") < 5L, CriteriaOperator.LT, 5L);

    [Fact]
    public void Less_String() => AssertBinary(new Criteria("A") < "x", CriteriaOperator.LT, "x");

    [Fact]
    public void Less_Double() => AssertBinary(new Criteria("A") < 5.5, CriteriaOperator.LT, 5.5);

    [Fact]
    public void Less_Decimal() => AssertBinary(new Criteria("A") < 5.5m, CriteriaOperator.LT, 5.5m);

    [Fact]
    public void Less_DateTime() => AssertBinary(new Criteria("A") < testDate, CriteriaOperator.LT, testDate);

    [Fact]
    public void Less_Guid() => AssertBinary(new Criteria("A") < testGuid, CriteriaOperator.LT, testGuid);

    [Fact]
    public void Less_Enum() => AssertBinary(new Criteria("A") < TestEnum.A, CriteriaOperator.LT, TestEnum.A);

    // <= overloads

    [Fact]
    public void LessEqual_BaseCriteria() => AssertOp(new Criteria("A") <= new Criteria("B"), CriteriaOperator.LE, "B");

    [Fact]
    public void LessEqual_Parameter() => AssertOpParam(new Criteria("A") <= new Parameter("@p1"), CriteriaOperator.LE, "@p1");

    [Fact]
    public void LessEqual_Int() => AssertBinary(new Criteria("A") <= 5, CriteriaOperator.LE, 5);

    [Fact]
    public void LessEqual_Long() => AssertBinary(new Criteria("A") <= 5L, CriteriaOperator.LE, 5L);

    [Fact]
    public void LessEqual_String() => AssertBinary(new Criteria("A") <= "x", CriteriaOperator.LE, "x");

    [Fact]
    public void LessEqual_Double() => AssertBinary(new Criteria("A") <= 5.5, CriteriaOperator.LE, 5.5);

    [Fact]
    public void LessEqual_Decimal() => AssertBinary(new Criteria("A") <= 5.5m, CriteriaOperator.LE, 5.5m);

    [Fact]
    public void LessEqual_DateTime() => AssertBinary(new Criteria("A") <= testDate, CriteriaOperator.LE, testDate);

    [Fact]
    public void LessEqual_Guid() => AssertBinary(new Criteria("A") <= testGuid, CriteriaOperator.LE, testGuid);

    [Fact]
    public void LessEqual_Enum() => AssertBinary(new Criteria("A") <= TestEnum.A, CriteriaOperator.LE, TestEnum.A);

    private static void AssertOp(BaseCriteria criteria, CriteriaOperator op, string expression)
    {
        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(op, binary.Operator);
        Assert.Equal(expression, ((Criteria)binary.RightOperand).Expression);
    }

    private static void AssertOpParam(BaseCriteria criteria, CriteriaOperator op, string name)
    {
        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(op, binary.Operator);
        Assert.Equal(name, Assert.IsType<ParamCriteria>(binary.RightOperand).Name);
    }

    // In / InStatement / NotIn special overloads

    [Fact]
    public void InStatement_SameAsIn()
    {
        var c = new Criteria("A");
        var statement = new Criteria("B");

        var result = Assert.IsType<BinaryCriteria>(c.InStatement(statement));

        Assert.Equal(CriteriaOperator.In, result.Operator);
        Assert.Same(statement, result.RightOperand);
    }

    [Fact]
    public void InStatement_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Criteria("A").InStatement(null));
    }

    [Fact]
    public void In_SqlQueryWithoutParent_WrapsInParentheses()
    {
        var query = new SqlQuery().Select("ID");
        query.From("TestTable").Where(new Criteria("ID") == 1).Dialect(new SqlServer2012Dialect());

        var result = Assert.IsType<BinaryCriteria>(new Criteria("ID").In(query));

        Assert.Equal(CriteriaOperator.In, result.Operator);
        Assert.Contains("SELECT", ((Criteria)result.RightOperand).Expression, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void In_SqlQueryWithParent_UsesRawStatement()
    {
        var query = new SqlQuery();
        var sub = query.SubQuery();

        var result = Assert.IsType<BinaryCriteria>(new Criteria("ID").In(sub));

        Assert.Equal(CriteriaOperator.In, result.Operator);
    }

    [Fact]
    public void NotIn_SingleCollectionValue_WrapsCollectionAsSingleOperand()
    {
        var result = Assert.IsType<BinaryCriteria>(new Criteria("A").NotIn(new object[] { new[] { "x", "y" } }));

        Assert.Equal(CriteriaOperator.NotIn, result.Operator);
        Assert.Equal(new[] { "x", "y" }, Assert.IsType<ValueCriteria>(result.RightOperand).Value);
    }

    [Fact]
    public void NotIn_SqlQuery_WrapsInParentheses()
    {
        var query = new SqlQuery().Select("ID");
        query.From("TestTable").Where(new Criteria("ID") == 1).Dialect(new SqlServer2012Dialect());

        var result = Assert.IsType<BinaryCriteria>(new Criteria("ID").NotIn(query));

        Assert.Equal(CriteriaOperator.NotIn, result.Operator);
    }
}
