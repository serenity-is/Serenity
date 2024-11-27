
namespace Serenity.Data;

/// <summary>
/// Sql Server 2008 dialect.
/// </summary>
/// <seealso cref="SqlServer2005Dialect" />
public class SqlServer2008Dialect : SqlServer2005Dialect
{
    /// <summary>
    /// The shared instance of SqlServer2008 dialect.
    /// </summary>
    public static new readonly ISqlDialect Instance = new SqlServer2008Dialect();

    /// <summary>
    /// Gets a value indicating whether use datetime2 type.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use datetime2; otherwise, <c>false</c>.
    /// </value>
    public override bool UseDateTime2 => true;
}