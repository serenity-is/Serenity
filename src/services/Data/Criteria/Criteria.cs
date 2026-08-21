namespace Serenity.Data;

/// <summary>
/// An object that is used to create criteria by employing the operator overloading
/// features of the C# language, instead of using string based criteria.
/// </summary>
public class Criteria : BaseCriteria
{
    /// <summary>
    /// An empty criteria instance.
    /// </summary>
    public static readonly BaseCriteria Empty = new Criteria();


    /// <summary>
    /// The false criteria instance (0 = 1).
    /// </summary>
    public static readonly BaseCriteria False = new Criteria("0=1");


    /// <summary>
    /// The true criteria instance (1 = 1).
    /// </summary>
    public static readonly BaseCriteria True = new Criteria("1=1");

    private readonly string expression;
    
    /// <summary>
    /// Gets a reference to the <see cref="IField"/> object passed to the constructor.
    /// </summary>
    public static IField Field { get; private set; }

    /// <summary>
    /// Creates an empty criteria.
    /// </summary>
    private Criteria()
    {
    }

    /// <summary>
    /// Creates a new criteria with the given condition. This condition is usually a
    /// field name, but it can also be a pre-generated criteria text.
    /// </summary>
    /// <remarks>
    /// Usually used like: <c>new Criteria("fieldname") >= 5</c>.
    /// </remarks>
    /// <param name="expression">
    /// A field name or criteria condition (can be <c>null</c>).
    /// </param>
    public Criteria(string expression)
    {
        this.expression = expression;
    }

    /// <summary>
    /// Creates a new criteria that contains the field name of the metafield.
    /// </summary>
    /// <param name="field">
    /// The field (required).
    /// </param>
    public Criteria(IField field)
    {
        Field = field ?? throw new ArgumentNullException(nameof(field));
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
        expression = alias + "." + SqlSyntax.AutoBracketValid(field, dialect: null);
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

        expression = joinNumber.TableAliasDot() + SqlSyntax.AutoBracketValid(field, dialect: null);
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
        Field = field;
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
        Field = field;
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
        Field = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// a custom expression while keeping reference to the provided field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <param name="expression">Custom expression</param>
    public Criteria(IField field, string expression)
        : this(expression)
    {
        Field = field ?? throw new ArgumentNullException(nameof(field));
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
    /// Creates a new criteria containing the field name in brackets.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>A new criteria with the field name in brackets.</returns>
    /// <exception cref="ArgumentNullException">fieldName is null or empty string.</exception>
    public static Criteria Bracket(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
            throw new ArgumentNullException("fieldName");

        return new Criteria("[" + fieldName + "]");
    }

    /// <summary>
    /// Creates a new EXISTS criteria.
    /// </summary>
    /// <param name="query">
    /// The expression.
    /// </param>
    /// <returns>A new EXISTS criteria.</returns>
    public static BaseCriteria Exists(ISqlQuery query)
    {
        return new UnaryCriteria(CriteriaOperator.Exists, new Criteria(query));
    }

    /// <summary>
    /// Creates a new EXISTS criteria.
    /// </summary>
    /// <param name="expression">
    /// The expression.
    /// </param>
    /// <returns>A new EXISTS criteria.</returns>
    public static BaseCriteria Exists(string expression)
    {
        return new UnaryCriteria(CriteriaOperator.Exists, new Criteria(expression));
    }

    /// <summary>
    /// Gets whether the criteria is empty.
    /// </summary>
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