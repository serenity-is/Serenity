namespace Serenity.Data;

public class UnaryCriteriaTests
{
    [Fact]
    public void Constructor_NullOperand_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new UnaryCriteria(CriteriaOperator.Paren, null!));
    }

    [Fact]
    public void Constructor_BinaryOperators_ThrowArgumentOutOfRange()
    {
        var operand = new Criteria("x");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UnaryCriteria(CriteriaOperator.AND, operand));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UnaryCriteria(CriteriaOperator.OR, operand));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UnaryCriteria(CriteriaOperator.XOR, operand));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UnaryCriteria(CriteriaOperator.EQ, operand));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new UnaryCriteria(CriteriaOperator.NotLike, operand));
    }

    [Fact]
    public void Constructor_AcceptsAllUnaryOperators()
    {
        var unaryOperators = new[]
        {
            CriteriaOperator.Paren, CriteriaOperator.Not, CriteriaOperator.IsNull,
            CriteriaOperator.IsNotNull, CriteriaOperator.Exists
        };

        foreach (var op in unaryOperators)
        {
            var criteria = new UnaryCriteria(op, new Criteria("x"));

            Assert.Equal(op, criteria.Operator);
        }
    }

    [Fact]
    public void ToString_Paren_WrapsOperand()
    {
        Assert.Equal("(x)", new UnaryCriteria(CriteriaOperator.Paren, new Criteria("x")).ToString());
    }

    [Fact]
    public void ToString_Not_PrefixesNot()
    {
        Assert.Equal("NOT (x)", new UnaryCriteria(CriteriaOperator.Not, new Criteria("x")).ToString());
    }

    [Fact]
    public void ToString_IsNull()
    {
        Assert.Equal("x IS NULL", new UnaryCriteria(CriteriaOperator.IsNull, new Criteria("x")).ToString());
    }

    [Fact]
    public void ToString_IsNotNull()
    {
        Assert.Equal("x IS NOT NULL", new UnaryCriteria(CriteriaOperator.IsNotNull, new Criteria("x")).ToString());
    }

    [Fact]
    public void ToString_Exists_WrapsOperand()
    {
        Assert.Equal("EXISTS (x)", new UnaryCriteria(CriteriaOperator.Exists, new Criteria("x")).ToString());
    }

    [Fact]
    public void ToString_Exists_WithParenthesizedExpression_DoesNotDoubleParenthesize()
    {
        // Operand expression already starts with "(" and ends with ")", so no extra
        // parentheses are appended (Sqlite does not like double parentheses in EXISTS).
        Assert.Equal("EXISTS (a)", new UnaryCriteria(CriteriaOperator.Exists, new Criteria("(a)")).ToString());
        Assert.Equal("EXISTS (a)", Criteria.Exists("(a)").ToString());
    }

    [Fact]
    public void Properties_ReturnOperatorAndOperand()
    {
        var operand = new Criteria("x");

        var criteria = new UnaryCriteria(CriteriaOperator.Not, operand);

        Assert.Equal(CriteriaOperator.Not, criteria.Operator);
        Assert.Same(operand, criteria.Operand);
    }
}
