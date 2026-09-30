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
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        if (op < CriteriaOperator.AND || op > CriteriaOperator.NotLike)
            throw new ArgumentOutOfRangeException(nameof(op));

        this.left = left;
        this.right = right;
        this.op = op;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryCriteria"/> class
    /// with a LIKE escape character.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="op">The operator. Must be Like or NotLike when likeEscapeChar is set.</param>
    /// <param name="right">The right operand (the LIKE pattern).</param>
    /// <param name="likeEscapeChar">The LIKE escape character, appended as ESCAPE 'x'.
    /// Retained only when the pattern contains it; otherwise omitted as inert.</param>
    /// <exception cref="ArgumentNullException">
    /// Left or right operand is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">Operator is not a binary one.</exception>
    /// <exception cref="ArgumentException">LikeEscapeChar is set with an operator other than
    /// Like / NotLike, or the escape character itself is a LIKE wildcard (%, _)
    /// or a quote.</exception>
    public BinaryCriteria(BaseCriteria left, CriteriaOperator op, BaseCriteria right, char likeEscapeChar)
        : this(left, op, right)
    {
        if (op != CriteriaOperator.Like && op != CriteriaOperator.NotLike)
            throw new ArgumentException("LikeEscapeChar can only be used with Like / NotLike operators.", nameof(likeEscapeChar));

        if (likeEscapeChar == '%' || likeEscapeChar == '_' || likeEscapeChar == '\'')
            throw new ArgumentException("Escape character can't be a LIKE wildcard (%, _) or a quote.", nameof(likeEscapeChar));

        // Retain the escape char only when the pattern contains it; otherwise the
        // ESCAPE clause would be inert, so omit it to render (and serialize) exactly
        // like a plain Like. Unknown pattern shapes keep it (explicit intent wins).
        var pattern = right;
        if (pattern is UpperFunctionCriteria upper &&
            upper.Arguments is { Length: 1 } args)
            pattern = args[0];

        if (pattern is not ValueCriteria { Value: string mask } ||
            mask.Contains(likeEscapeChar))
            LikeEscapeChar = likeEscapeChar;
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
        if (LikeEscapeChar is char escapeChar)
        {
            sb.Append(" ESCAPE '");
            sb.Append(escapeChar);
            sb.Append('\'');
        }
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

    /// <summary>
    /// Gets the LIKE escape character, or null when no ESCAPE clause is rendered.
    /// Only set for Like / NotLike operators.
    /// </summary>
    /// <value>
    /// The escape character, or null.
    /// </value>
    public char? LikeEscapeChar { get; }
}