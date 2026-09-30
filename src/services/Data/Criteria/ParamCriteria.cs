namespace Serenity.Data;

/// <summary>
/// A criteria object containing a parameter name
/// </summary>
/// <seealso cref="BaseCriteria" />
/// <remarks>
/// Initializes a new instance of the <see cref="ParamCriteria"/> class.
/// </remarks>
/// <param name="name">The parameter name. Should start with @.</param>
/// <exception cref="ArgumentNullException">name is null or empty</exception>
/// <exception cref="ArgumentOutOfRangeException">name doesn't start with "@",
/// or has nothing but whitespace after it.</exception>
public class ParamCriteria(string name) : BaseCriteria
{
    private readonly string name = Parameter.CheckName(name);

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
    /// Gets the parameter name including the @ symbol.
    /// </summary>
    /// <value>
    /// The parameter name.
    /// </value>
    public string Name => name;

}