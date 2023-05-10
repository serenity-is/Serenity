namespace Serenity.Data;

/// <summary>
///   Used to define aliases like (T0).</summary>
public class Alias : IAlias
{
    /// <summary>
    /// Static t0 alias
    /// </summary>
    public static readonly Alias T0 = new Alias(0);

    /// <summary>
    /// Static t1 alias
    /// </summary>
    public static readonly Alias T1 = new Alias(1);

    /// <summary>
    /// Static t2 alias
    /// </summary>
    public static readonly Alias T2 = new Alias(2);

    /// <summary>
    /// Static t3 alias
    /// </summary>
    public static readonly Alias T3 = new Alias(3);

    /// <summary>
    /// Static t4 alias
    /// </summary>
    public static readonly Alias T4 = new Alias(4);

    /// <summary>
    /// Static t5 alias
    /// </summary>
    public static readonly Alias T5 = new Alias(5);

    /// <summary>
    /// Static t6 alias
    /// </summary>
    public static readonly Alias T6 = new Alias(6);

    /// <summary>
    /// Static t7 alias
    /// </summary>
    public static readonly Alias T7 = new Alias(7);

    /// <summary>
    /// Static t8 alias
    /// </summary>
    public static readonly Alias T8 = new Alias(8);

    /// <summary>
    /// Static t9 alias
    /// </summary>
    public static readonly Alias T9 = new Alias(9);

    private readonly string alias;
    private readonly string aliasDot;
    private readonly string table;

    /// <summary>
    /// Initializes a new instance of the <see cref="Alias"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    public Alias(int alias)
    {
        this.alias = alias.TableAlias();
        aliasDot = alias.TableAliasDot();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alias"/> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <exception cref="ArgumentNullException">alias</exception>
    public Alias(string alias)
    {
        if (string.IsNullOrEmpty(alias))
            throw new ArgumentNullException("alias");

        this.alias = alias;
        aliasDot = alias + ".";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alias"/> class.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="alias">The alias.</param>
    public Alias(string table, int alias)
        : this(alias)
    {
        this.table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alias"/> class.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="alias">The alias.</param>
    public Alias(string table, string alias)
        : this(alias)
    {
        this.table = table;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name => alias;

    /// <summary>
    /// Gets the name dot.
    /// </summary>
    /// <value>
    /// The name dot.
    /// </value>
    public string NameDot => aliasDot;

    /// <summary>
    /// Gets the table.
    /// </summary>
    /// <value>
    /// The table.
    /// </value>
    public string Table => table;

    /// <summary>
    /// Gets the prefixed expression with the specified field name.
    /// </summary>
    /// <value>
    /// The <see cref="string"/>.
    /// </value>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>Expression like t0.fieldName</returns>
    public string this[string fieldName]
    {
        get { return aliasDot + fieldName; }
    }

    /// <summary>
    /// Gets the prefixed expression with the specified field.
    /// </summary>
    /// <value>
    /// The <see cref="string"/>.
    /// </value>
    /// <param name="field">The field.</param>
    /// <returns>Expression like t0.fieldName</returns>
    /// <exception cref="ArgumentNullException">field</exception>
    public string this[IField field]
    {
        get
        {
            if (field == null)
                throw new ArgumentNullException("field");

            return aliasDot + field.Name;
        }
    }

    /// <summary>
    /// Gets a criteria containing prefixed field.
    /// Only here for backward compatibility.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>Criteria containing prefixed field</returns>
    public Criteria _(string field)
    {
        return new Criteria(this[field]);
    }

    /// <summary>
    /// Gets a criteria containing prefixed field.
    /// Only here for backward compatibility.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>Criteria containing prefixed field</returns>
    public Criteria _(IField field)
    {
        return new Criteria(this[field]);
    }

    /// <summary>
    /// Gets a criteria containing prefixed field.
    /// Only here for backward compatibility.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>
    /// String containing prefixed field
    /// </returns>
    public static string operator +(Alias alias, string fieldName)
    {
        return alias.aliasDot + fieldName;
    }

    /// <summary>
    /// Gets a criteria containing prefixed field.
    /// Only here for backward compatibility.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="field">The field.</param>
    /// <returns>
    /// String containing prefixed field
    /// </returns>
    /// <exception cref="ArgumentNullException">field</exception>
    public static string operator +(Alias alias, IField field)
    {
        if (field == null)
            throw new ArgumentNullException("field");

        return alias.aliasDot + field.Name;
    }
}