
namespace Serenity.Data;

/// <summary>
/// SQL dialect for Oracle 12c, which uses OFFSET FETCH instead of ROWNUM.
/// </summary>
public class Oracle12cDialect : OracleDialect
{
    /// <summary>
    /// The shared instance of Oracle12cDialect.
    /// </summary>
    public static new readonly ISqlDialect Instance = new Oracle12cDialect();

    /// <inheritdoc/>
    public override bool CanUseOffsetFetch => true;

    /// <inheritdoc/>
    public override string OffsetFormat => " OFFSET {0} ROWS";

    /// <inheritdoc/>
    public override string OffsetFetchFormat => " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY";
}