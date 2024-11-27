namespace Serenity.Data;

/// <summary>
/// A criteria object containing a parameter name
/// </summary>
/// <seealso cref="BaseCriteria" />
public class ParamCriteria : BaseCriteria
{
    private readonly string name;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamCriteria"/> class.
    /// </summary>
    /// <param name="name">The parameter name. Should not start with @.</param>
    /// <exception cref="ArgumentNullException">name is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">name starts with @.</exception>
    public ParamCriteria(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");

        if (!name.StartsWith("@"))
            throw new ArgumentOutOfRangeException("name");

        this.name = name;
    }

    /// <summary>
    /// Converts the criteria to string.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="query">The query.</param>
    public override void ToString(StringBuilder sb, IQueryWithParams query)
    {
        sb.Append(name);
    }

    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    /// <value>
    /// The parameter name.
    /// </value>
    public string Name => name;
}