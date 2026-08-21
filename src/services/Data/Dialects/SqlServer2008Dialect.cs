
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

    /// <inheritdoc/>
    public override bool UseDateTime2 => true;
}