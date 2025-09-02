
namespace Serenity.Data;

/// <summary>
/// Sql Server 2012 dialect.
/// </summary>
/// <seealso cref="SqlServer2008Dialect" />
public class SqlServer2012Dialect : SqlServer2008Dialect
{
    /// <summary>
    /// The shared instance of SqlServer2012Dialect.
    /// </summary>
    public static new readonly ISqlDialect Instance = new SqlServer2012Dialect();

    /// <inheritdoc/>
    public override bool CanUseConcat => true;

    /// <summary>
    /// Gets a value indicating whether the server supports OFFSET FETCH.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
    /// </value>
    public override bool CanUseOffsetFetch => true;

    /// <summary>
    /// Gets the format for OFFSET only statements.
    /// </summary>
    /// <value>
    /// The offset format.
    /// </value>
    public override string OffsetFormat => " OFFSET {0} ROWS";

    /// <summary>
    /// Gets the format for OFFSET FETCH statements.
    /// </summary>
    /// <value>
    /// The offset fetch format.
    /// </value>
    public override string OffsetFetchFormat => " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";
}