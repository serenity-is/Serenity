namespace Serenity.Data;

public class BinaryCriteriaTests
{
    [Fact]
    public void Constructor_NullLeftOperand_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BinaryCriteria(null!, CriteriaOperator.AND, new Criteria("B")));
    }

    [Fact]
    public void Constructor_NullRightOperand_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BinaryCriteria(new Criteria("A"), CriteriaOperator.AND, null!));
    }

    [Fact]
    public void Constructor_UnaryOperators_ThrowArgumentOutOfRange()
    {
        var left = new Criteria("A");
        var right = new Criteria("B");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BinaryCriteria(left, CriteriaOperator.Paren, right));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BinaryCriteria(left, CriteriaOperator.Not, right));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BinaryCriteria(left, CriteriaOperator.IsNull, right));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BinaryCriteria(left, CriteriaOperator.IsNotNull, right));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BinaryCriteria(left, CriteriaOperator.Exists, right));
    }

    [Fact]
    public void Constructor_AcceptsAllBinaryOperators()
    {
        var binaryOperators = new[]
        {
            CriteriaOperator.AND, CriteriaOperator.OR, CriteriaOperator.XOR,
            CriteriaOperator.EQ, CriteriaOperator.NE, CriteriaOperator.GT, CriteriaOperator.GE,
            CriteriaOperator.LT, CriteriaOperator.LE, CriteriaOperator.In, CriteriaOperator.NotIn,
            CriteriaOperator.Like, CriteriaOperator.NotLike
        };

        foreach (var op in binaryOperators)
        {
            var criteria = new BinaryCriteria(new Criteria("A"), op, new Criteria("B"));

            Assert.Equal(op, criteria.Operator);
        }
    }

    [Fact]
    public void ToString_LogicalOperators()
    {
        Assert.Equal("(A AND B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.AND, new Criteria("B")).ToString());
        Assert.Equal("(A OR B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.OR, new Criteria("B")).ToString());
        Assert.Equal("(A XOR B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.XOR, new Criteria("B")).ToString());
    }

    [Fact]
    public void ToString_ComparisonOperators()
    {
        Assert.Equal("(A = B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.EQ, new Criteria("B")).ToString());
        Assert.Equal("(A != B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.NE, new Criteria("B")).ToString());
        Assert.Equal("(A > B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.GT, new Criteria("B")).ToString());
        Assert.Equal("(A >= B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.GE, new Criteria("B")).ToString());
        Assert.Equal("(A < B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.LT, new Criteria("B")).ToString());
        Assert.Equal("(A <= B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.LE, new Criteria("B")).ToString());
    }

    [Fact]
    public void ToString_InAndLikeOperators()
    {
        Assert.Equal("(A IN B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.In, new Criteria("B")).ToString());
        Assert.Equal("(A NOT IN B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.NotIn, new Criteria("B")).ToString());
        Assert.Equal("(A LIKE B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.Like, new Criteria("B")).ToString());
        Assert.Equal("(A NOT LIKE B)", new BinaryCriteria(
            new Criteria("A"), CriteriaOperator.NotLike, new Criteria("B")).ToString());
    }

    [Fact]
    public void Properties_ReturnOperatorAndOperands()
    {
        var left = new Criteria("A");
        var right = new Criteria("B");

        var criteria = new BinaryCriteria(left, CriteriaOperator.EQ, right);

        Assert.Equal(CriteriaOperator.EQ, criteria.Operator);
        Assert.Same(left, criteria.LeftOperand);
        Assert.Same(right, criteria.RightOperand);
    }
}
