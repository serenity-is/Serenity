namespace Serenity.Data.Mapping;

/// <summary>
/// Cast expression attribute, which converts a value to one of the types that have an equivalent in all supported
/// databases: <see cref="DbType.Int32"/>, <see cref="DbType.Int64"/>, <see cref="DbType.Decimal"/> with
/// <see cref="Precision"/> and <see cref="Scale"/>, <see cref="DbType.String"/> with <see cref="Size"/>, and
/// <see cref="DbType.Date"/>, which drops the time. Converting a number with a fraction to an integer truncates it in
/// SQL Server and SQLite, while it rounds it in the others. SQLite has no decimal type, so a decimal is a REAL there.
/// </summary>
/// <param name="expression">An expression that returns the value to convert.</param>
/// <param name="type">The type to convert to.</param>
/// <exception cref="ArgumentNullException">The expression is null.</exception>
public class CastAttribute(object expression, DbType type) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        var expression = ToString(Expression, dialect);
        var serverType = dialect.ServerType;

        if (Type == DbType.Date)
        {
            return serverType switch
            {
                // CAST(x AS DATE) gives a number in SQLite, as it is only a type affinity
                nameof(ServerType.Sqlite) => $"DATE({expression})",
                // a DATE has a time in Oracle
                nameof(ServerType.Oracle) => $"TRUNC(CAST({expression} AS DATE))",
                _ => $"CAST({expression} AS DATE)"
            };
        }

        var typeName = Type switch
        {
            DbType.Int32 => serverType switch
            {
                nameof(ServerType.MySql) => "SIGNED",
                nameof(ServerType.Oracle) => "NUMBER(10)",
                nameof(ServerType.SqlServer) => "INT",
                _ => "INTEGER"
            },
            DbType.Int64 => serverType switch
            {
                nameof(ServerType.MySql) => "SIGNED",
                nameof(ServerType.Oracle) => "NUMBER(19)",
                nameof(ServerType.Sqlite) => "INTEGER",
                _ => "BIGINT"
            },
            DbType.Decimal => serverType switch
            {
                // DECIMAL is only a type affinity in SQLite, which keeps a whole number an integer, so 7 / 4 would be 1
                nameof(ServerType.Sqlite) => "REAL",
                nameof(ServerType.Oracle) => FormattableString.Invariant($"NUMBER({Precision}, {Scale})"),
                _ => FormattableString.Invariant($"DECIMAL({Precision}, {Scale})")
            },
            DbType.String => serverType switch
            {
                // MySQL doesn't accept VARCHAR in CAST
                nameof(ServerType.MySql) => FormattableString.Invariant($"CHAR({Size})"),
                nameof(ServerType.Oracle) => FormattableString.Invariant($"VARCHAR2({Size} CHAR)"),
                nameof(ServerType.Sqlite) => "TEXT",
                nameof(ServerType.SqlServer) => FormattableString.Invariant($"NVARCHAR({Size})"),
                _ => FormattableString.Invariant($"VARCHAR({Size})")
            },
            _ => throw new NotSupportedException($"Casting to {Type} is not supported.")
        };

        return $"CAST({expression} AS {typeName})";
    }

    /// <summary>
    /// The expression to convert.
    /// </summary>
    public object Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>
    /// The type to convert to.
    /// </summary>
    public DbType Type { get; } = type;

    /// <summary>
    /// The number of characters for <see cref="DbType.String"/>, 255 by default.
    /// </summary>
    public int Size { get; set; } = 255;

    /// <summary>
    /// The number of digits for <see cref="DbType.Decimal"/>, 18 by default.
    /// </summary>
    public int Precision { get; set; } = 18;

    /// <summary>
    /// The number of digits after the decimal point for <see cref="DbType.Decimal"/>, 2 by default.
    /// </summary>
    public int Scale { get; set; } = 2;
}
