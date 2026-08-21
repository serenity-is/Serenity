
namespace Serenity.Data;

/// <summary>
/// Sql Server 2005 dialect.
/// </summary>
/// <seealso cref="SqlServer2000Dialect" />
public class SqlServer2005Dialect : SqlServer2000Dialect
{
    /// <summary>
    /// The shared instance of SqlServer2005Dialect.
    /// </summary>
    public static new readonly ISqlDialect Instance = new SqlServer2005Dialect();

    /// <inheritdoc/>
    public override bool CanUseRowNumber => true;
}