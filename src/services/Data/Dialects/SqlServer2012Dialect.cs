
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

    /// <inheritdoc/>
    public override bool CanUseOffsetFetch => true;

    /// <inheritdoc/>
    public override string OffsetFormat => " OFFSET {0} ROWS";

    /// <inheritdoc/>
    public override string OffsetFetchFormat => " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";
}