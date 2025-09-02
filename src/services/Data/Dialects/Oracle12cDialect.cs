
namespace Serenity.Data;

/// <summary>
/// Oracle12cDialect which uses OFFSET FETCH instead of ROWNUM
/// </summary>
public class Oracle12cDialect : OracleDialect
{
    /// <summary>
    /// The shared instance of Oracle12cDialect.
    /// </summary>
    public static new readonly ISqlDialect Instance = new Oracle12cDialect();

    /// <summary>
    /// Gets a value indicating whether the server supports OFFSET FETCH.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
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