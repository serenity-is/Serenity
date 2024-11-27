namespace Serenity.Data;

/// <summary>
/// Binary criteria object, which has two operands and a operator.
/// </summary>
/// <seealso cref="BaseCriteria" />
public class BinaryCriteria : BaseCriteria
{
    private readonly BaseCriteria left;
    private readonly BaseCriteria right;
    private readonly CriteriaOperator op;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryCriteria"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="op">The operator.</param>
    /// <param name="right">The right operand.</param>
    /// <exception cref="ArgumentNullException">
    /// Left or right operand is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">Operator is not a binary one.</exception>
    public BinaryCriteria(BaseCriteria left, CriteriaOperator op, BaseCriteria right)
    {
        if (left is null)
            throw new ArgumentNullException("left");

        if (right is null)
            throw new ArgumentNullException("right");

        if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
            throw new ArgumentOutOfRangeException("op");

        this.left = left;
        this.right = right;
        this.op = op;
    }

    /// <summary>
    /// Converts the criteria to string in a string builder, 
    /// while adding its params to the target query.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append('(');
        left.ToString(sb, query);
        sb.Append(opText[(int)op - (int)CriteriaOperator.AND]);
        right.ToString(sb, query);
        sb.Append(')');
    }

    private static readonly string[] opText =
    [
        " AND ",
        " OR ",
        " XOR ",
        " = ",
        " != ",
        " > ",
        " >= ",
        " < ",
        " <= ",
        " IN ",
        " NOT IN ",
        " LIKE ",
        " NOT LIKE "
    ];

    /// <summary>
    /// Gets the operator.
    /// </summary>
    /// <value>
    /// The operator.
    /// </value>
    public CriteriaOperator Operator => op;

    /// <summary>
    /// Gets the left operand.
    /// </summary>
    /// <value>
    /// The left operand.
    /// </value>
    public BaseCriteria LeftOperand => left;

    /// <summary>
    /// Gets the right operand.
    /// </summary>
    /// <value>
    /// The right operand.
    /// </value>
    public BaseCriteria RightOperand => right;
}