namespace Serenity.Data;

/// <summary>
/// Abstraction for SQL dialect, e.g. syntax for different SQL server types and connection settings.
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// Gets a value indicating whether the server supports CONCAT.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports CONCAT function; otherwise, <c>false</c>.
    /// </value>
    public bool CanUseConcat => true;

    /// <summary>
    /// Gets a value indicating whether the server supports OFFSET FETCH.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
    /// </value>
    bool CanUseOffsetFetch { get; }

    /// <summary>
    /// Gets a value indicating whether the server supports ROWNUMBER.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports ROWNUMBER; otherwise, <c>false</c>.
    /// </value>
    bool CanUseRowNumber { get; }

    /// <summary>
    /// Gets a value indicating whether the server supports SKIP keyword (or a variation of it).
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports a variation of SKIP keyword; otherwise, <c>false</c>.
    /// </value>
    bool CanUseSkipKeyword { get; }

    /// <summary>
    /// Gets the close quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The close quote.
    /// </value>
    char CloseQuote { get; }

    /// <summary>
    /// Gets the CONCAT operator keyword.
    /// </summary>
    /// <value>
    /// The CONCAT operator keyword.
    /// </value>
    string ConcatOperator { get; }

    /// <summary>
    /// Gets the date format.
    /// </summary>
    /// <value>
    /// The date format.
    /// </value>
    string DateFormat { get; }

    /// <summary>
    /// Gets the date time format.
    /// </summary>
    /// <value>
    /// The date time format.
    /// </value>
    string DateTimeFormat { get; }

    /// <summary>
    /// Gets a value indicating whether the LIKE operator is case sensitive.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the LIKE operator is sensitive; otherwise, <c>false</c>.
    /// </value>
    bool IsLikeCaseSensitive { get; }

    /// <summary>
    /// Gets a value indicating whether the server supports multiple resultsets.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server supports multiple resultsets; otherwise, <c>false</c>.
    /// </value>
    bool MultipleResultsets { get; }

    /// <summary>
    /// Gets a value indicating whether the server needs EXECUTE BLOCK statement.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server needs EXECUTE BLOCK statement; otherwise, <c>false</c>.
    /// </value>
    bool NeedsExecuteBlockStatement { get; }

    /// <summary>
    /// Gets a value indicating whether the server needs a workaround to handle Boolean values false/true.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server needs a workaround to handle Boolean values false/true; otherwise, <c>false</c>.
    /// </value>
    bool NeedsBoolWorkaround { get; }

    /// <summary>
    /// Gets the format for OFFSET only statements.
    /// </summary>
    /// <value>
    /// The offset format.
    /// </value>
    string OffsetFormat { get; }

    /// <summary>
    /// Gets the format for OFFSET FETCH statements.
    /// </summary>
    /// <value>
    /// The offset fetch format.
    /// </value>
    string OffsetFetchFormat { get; }

    /// <summary>
    /// Gets the open quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The open quote.
    /// </value>
    char OpenQuote { get; }

    /// <summary>
    /// Quotes the column alias. This usually calls QuoteIdentifier except for Oracle.
    /// </summary>
    /// <param name="s">The column alias.</param>
    /// <returns>Quoted column alias</returns>
    string QuoteColumnAlias(string s);

    /// <summary>
    /// Quotes the identifier.
    /// </summary>
    /// <param name="s">The identifier.</param>
    /// <returns>Quoted identifier</returns>
    string QuoteIdentifier(string s);

    /// <summary>
    /// Quotes the unicode string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns></returns>
    string QuoteUnicodeString(string s);

    /// <summary>
    /// Gets the SCOPE IDENTITY expression.
    /// </summary>
    /// <value>
    /// The SCOPE IDENTITY expression.
    /// </value>
    string ScopeIdentityExpression { get; }

    /// <summary>
    /// Gets the type of the server.
    /// </summary>
    /// <value>
    /// The type of the server.
    /// </value>
    string ServerType { get; }

    /// <summary>
    /// Gets the skip keyword.
    /// </summary>
    /// <value>
    /// The skip keyword.
    /// </value>
    string SkipKeyword { get; }

    /// <summary>
    /// Gets the take keyword.
    /// </summary>
    /// <value>
    /// The take keyword.
    /// </value>
    string TakeKeyword { get; }

    /// <summary>
    /// Gets the time format.
    /// </summary>
    /// <value>
    /// The time format.
    /// </value>
    string TimeFormat { get; }

    /// <summary>
    /// Gets the union keyword for specified union type.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>Union keyword</returns>
    string UnionKeyword(SqlUnionType unionType);

    /// <summary>
    /// Gets a value indicating whether use datetime2 type.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use datetime2; otherwise, <c>false</c>.
    /// </value>
    bool UseDateTime2 { get; }

    /// <summary>
    /// Gets a value indicating whether to use returning identity.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should use returning identity; otherwise, <c>false</c>.
    /// </value>
    bool UseReturningIdentity { get; }

    /// <summary>
    /// Gets a value indicating whether use returning into variable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use returning into variable; otherwise, <c>false</c>.
    /// </value>
    bool UseReturningIntoVar { get; }

    /// <summary>
    /// Gets a value indicating whether to use scope identity.
    /// </summary>
    /// <value>
    ///   <c>true</c> if to use scope identity; otherwise, <c>false</c>.
    /// </value>
    bool UseScopeIdentity { get; }

    /// <summary>
    /// Gets a value indicating whether to use TAKE at end.
    /// </summary>
    /// <value>
    ///   <c>true</c> if to use TAKE at end; otherwise, <c>false</c>.
    /// </value>
    bool UseTakeAtEnd { get; }

    /// <summary>
    /// Gets a value indicating whether ROWNUM.
    /// </summary>
    /// <value>
    ///   <c>true</c> if can use ROWNUM; otherwise, <c>false</c>.
    /// </value>
    bool UseRowNum { get; }

    /// <summary>
    /// Gets the parameter prefix character.
    /// </summary>
    /// <value>
    /// The parameter prefix character.
    /// </value>
    char ParameterPrefix { get; }
}