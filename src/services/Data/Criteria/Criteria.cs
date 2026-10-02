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
    private readonly string? aliasDot;
    private readonly string? fieldName;
    
    /// <summary>
    /// Gets a reference to the <see cref="IField"/> object passed to the constructor.
    /// </summary>
    public IField? Field { get; private set; }

    /// <summary>
    /// Creates an empty criteria.
    /// </summary>
    private Criteria()
    {
        expression = "";
    }

    /// <summary>
    /// Creates a new criteria with the given condition. This condition is usually a
    /// field name, but it can also be a pre-generated criteria text.
    /// </summary>
    /// <remarks>
    /// Usually used like: <c>new Criteria("fieldname") >= 5</c>.
    /// </remarks>
    /// <param name="expression">
    /// A field name or criteria condition (cannot be <c>null</c>).
    /// </param>
    public Criteria(string expression)
    {
        this.expression = expression ?? throw new ArgumentNullException(nameof(expression));
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
        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException(nameof(alias));

        if (string.IsNullOrEmpty(field))
            throw new ArgumentNullException(nameof(field));        
            
        aliasDot = alias + ".";
        expression = aliasDot + SqlSyntax.AutoBracketValid(field, dialect: null);
        fieldName = field;
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
            throw new ArgumentNullException(nameof(field));

        ArgumentOutOfRangeException.ThrowIfNegative(joinNumber);
        aliasDot = joinNumber.TableAliasDot();
        expression = aliasDot + SqlSyntax.AutoBracketValid(field, dialect: null);
        fieldName = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "alias.field".
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">alias or field is null</exception>
    public Criteria(IAlias alias, IField field)
        : this((alias ?? throw new ArgumentNullException(nameof(alias))).Name,
               (field ?? throw new ArgumentNullException(nameof(field))).Name)
    {
        Field = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "alias.field".
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">alias is null</exception>
    public Criteria(IAlias alias, string field)
        : this((alias ?? throw new ArgumentNullException(nameof(alias))).Name, field)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "tjoinNumber.field"
    /// </summary>
    /// <param name="joinNumber">The join number.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">field is null</exception>
    public Criteria(int joinNumber, IField field)
        : this(joinNumber, (field ?? throw new ArgumentNullException(nameof(field))).Name)
    {
        Field = field;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Criteria"/> class containing
    /// an expression like "join.field".
    /// </summary>
    /// <param name="join">The join.</param>
    /// <param name="field">The field.</param>
    /// <exception cref="ArgumentNullException">field is null</exception>
    public Criteria(string join, IField field)
        : this(join, (field ?? throw new ArgumentNullException(nameof(field))).Name)
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
    /// <exception cref="ArgumentNullException">query is null</exception>
    /// <exception cref="InvalidOperationException">query is an independent
    /// query with auto parameters.</exception>
    /// <remarks>
    /// The query must share the same root query that this criteria will be used
    /// in (usually built via that query's SubQuery() method). An unrelated
    /// query's parameters are not available to the outer query, so embedding it
    /// may produce completely invalid results. Independent queries without
    /// parameters are safe to embed.
    /// </remarks>
    public Criteria(ISqlQuery query)
        // Rendered in the base call: auto parameters only materialize at render
        // time, so a freshly built subquery reports no autos until ToString runs.
        : this((query ?? throw new ArgumentNullException(nameof(query))).ToString()!)
    {
        query.ThrowIfUnsharedSubQueryWithAutoParams();
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
            throw new ArgumentNullException(nameof(fieldName));

        return new Criteria("[" + fieldName + "]");
    }

    /// <summary>
    /// Creates a new EXISTS criteria.
    /// </summary>
    /// <param name="query">
    /// The expression. Must share the same root query that the resulting
    /// criteria will be used in (usually built via that query's SubQuery()
    /// method); an unrelated query's parameters are not available to the
    /// outer query, so embedding it may produce completely invalid results.
    /// </param>
    /// <returns>A new EXISTS criteria.</returns>
    /// <exception cref="InvalidOperationException">query is an independent
    /// query with auto parameters.</exception>
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
    /// Creates a new EXISTS criteria from another criteria expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>A new EXISTS criteria.</returns>
    public static BaseCriteria Exists(BaseCriteria expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return new UnaryCriteria(CriteriaOperator.Exists, expression);
    }

    /// <summary>
    /// Escapes a literal value for use in a LIKE pattern with the given escape character.
    /// The escape character itself is escaped first, then %, _ and [.
    /// A ] outside a [...] class is literal on all supported dialects, so it is left as is.
    /// The result must be used with ESCAPE 'x' declared (see
    /// <see cref="BinaryCriteria.LikeEscapeChar"/>), otherwise the escape
    /// character matches itself instead of escaping.
    /// </summary>
    /// <param name="value">The literal value to escape.</param>
    /// <param name="escape">The escape character. Must not be %, _ or a quote. Default is '!'.</param>
    /// <returns>The escaped value, with no % affixes added.</returns>
    /// <exception cref="ArgumentNullException">value is null.</exception>
    /// <exception cref="ArgumentException">The escape character is a LIKE wildcard (%, _) or a quote.</exception>
    public static string EscapeLikeWildcards(string value, char escape = '!')
    {
        ArgumentNullException.ThrowIfNull(value);

        if (escape == '%' || escape == '_' || escape == '\'')
            throw new ArgumentException("Escape character can't be a LIKE wildcard (%, _) or a quote.", nameof(escape));

        if (value.Length == 0)
            return value;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c == escape || c == '%' || c == '_' || c == '[')
                sb.Append(escape);
            sb.Append(c);
        }
        return sb.ToString();
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
        ArgumentNullException.ThrowIfNull(sb);
        ArgumentNullException.ThrowIfNull(query);

        if (fieldName is string field)
        {
            sb.Append(aliasDot);
            sb.Append(SqlSyntax.AutoBracketValid(field, query.Dialect));
        }
        else
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
