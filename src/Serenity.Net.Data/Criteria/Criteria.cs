namespace Serenity.Data;

/// <summary>
///   An object that is used to create criterias by employing operator overloading 
///   features of C# language, instead of using string based criterias.</summary>
public class Criteria : BaseCriteria
{
    /// <summary>
    /// An empty criteria instance
    /// </summary>
    public static readonly BaseCriteria Empty = new Criteria();


    /// <summary>
    /// The false criteria instance (0 = 1)
    /// </summary>
    public static readonly BaseCriteria False = new Criteria("0=1");


    /// <summary>
    /// The true criteria instance (1 = 1)
    /// </summary>
    public static readonly BaseCriteria True = new Criteria("1=1");

    private readonly string expression;

    /// <summary>
    ///   Creates an empty criteria</summary>
    private Criteria()
    {
    }

    /// <summary>
    ///   Creates a new criteria with given condition. This condition is usually a 
    ///   field name, but it can also be a criteria text pre-generated.</summary>
    /// <remarks>
    ///   Usually used like: <c>new Criteria("fieldname") >= 5</c>.</remarks>
    /// <param name="expression">
    ///   A field name or criteria condition (can be null)</param>
    public Criteria(string expression)
    {
        this.expression = expression;
    }

    /// <summary>
    ///   Creates a new criteria that contains field name of the metafield.</summary>
    /// <param name="field">
    ///   Field (required).</param>
    public Criteria(IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        expression = field.Expression;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class
    /// containing an expression like "alias.field".
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">
    /// Field or alias is null or empty string.
    /// </exception>
    public Criteria(string alias, string field)
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException("field");

        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException("alias");
        expression = alias + "." + SqlSyntax.AutoBracketValid(field);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class
    /// containing an expression like "tjoinnumber.field" (t7.field).
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">field is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">joinNumber is less than zero</exception>
    public Criteria(int joinNumber, string field)
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException("field");

        if (joinNumber < 0)
            throw new ArgumentOutOfRangeException("joinNumber");

        expression = joinNumber.TableAliasDot() + SqlSyntax.AutoBracketValid(field);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "alias.field".
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    public Criteria(IAlias alias, IField field)
        : this(alias.Name, field.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "alias.field".
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    public Criteria(IAlias alias, string field)
        : this(alias.Name, field)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "tjoinNumber.field"
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    public Criteria(int joinNumber, IField field)
        : this(joinNumber, field.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "join.field".
    /// </summary>
    /// <param name="join">The join.</param>
    /// <param name="field">The field.</param>
    public Criteria(string join, IField field)
        : this(join, field.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// a query's string representation.
    /// </summary>
    /// <param name="query">The query.</param>
    public Criteria(ISqlQuery query)
        : this(query.ToString())
    {
    }


    /// <summary>
    /// Creates a new criteria containing field name in brackets.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">fieldName is null or empty string.</exception>
    public static Criteria Bracket(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
            throw new ArgumentNullException("fieldName");

        return new Criteria("[" + fieldName + "]");
    }

    /// <summary>
    ///   Creates a new EXISTS criteria</summary>
    /// <param name="query">
    ///   Expression</param>
    /// <returns></returns>
    public static BaseCriteria Exists(ISqlQuery query)
    {
        return new UnaryCriteria(CriteriaOperator.Exists, new Criteria(query));
    }

    /// <summary>
    ///   Creates a new EXISTS criteria</summary>
    /// <param name="expression">
    ///   Expression</param>
    /// <returns></returns>
    public static BaseCriteria Exists(string expression)
    {
        return new UnaryCriteria(CriteriaOperator.Exists, new Criteria(expression));
    }

    /// <summary>
    ///   Gets if criteria is empty.</summary>
    public override bool IsEmpty => string.IsNullOrEmpty(expression);

    /// <summary>
    /// Converts the criteria to its string representation while
    /// adding its parameters to the target query.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The target query to add params into.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append(expression);
    }

    /// <summary>
    /// Gets the criteria expression.
    /// </summary>
    /// <value>
    /// The raw criteria expression.
    /// </value>
    public string Expression => expression;
}