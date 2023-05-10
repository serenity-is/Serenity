namespace Serenity.Data;

/// <summary>
/// Set of criteria operator types
/// </summary>
public enum CriteriaOperator
{
    /// <summary>
    /// Parenthesis
    /// </summary>
    Paren,

    /// <summary>
    /// NOT operator
    /// </summary>
    Not,

    /// <summary>
    /// IS NULL operator
    /// </summary>
    IsNull,

    /// <summary>
    /// IS NOT NULL operator
    /// </summary>
    IsNotNull,

    /// <summary>
    /// EXISTS operator
    /// </summary>
    Exists,

    /// <summary>
    /// AND operator
    /// </summary>
    AND,

    /// <summary>
    /// OR operator
    /// </summary>
    OR,

    /// <summary>
    /// XOR operator
    /// </summary>
    XOR,

    /// <summary>
    /// Equal (=) operator
    /// </summary>
    EQ,

    /// <summary>
    /// Not Equal (&lt;&gt;) operator
    /// </summary>
    NE,

    /// <summary>
    /// Greater than (&gt;) operator
    /// </summary>
    GT,

    /// <summary>
    /// Greater Than or Equal To (&gt;=) operator
    /// </summary>
    GE,

    /// <summary>
    /// Less Than (&lt;) Operator
    /// </summary>
    LT,

    /// <summary>
    /// Less Than or Equal To (&lt;=) operator
    /// </summary>
    LE,

    /// <summary>
    /// IN operator
    /// </summary>
    In,

    /// <summary>
    /// NOT IN operator
    /// </summary>
    NotIn,

    /// <summary>
    /// LIKE operator
    /// </summary>
    Like,

    /// <summary>
    /// NOT LIKE operator
    /// </summary>
    NotLike,
}