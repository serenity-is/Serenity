namespace Serenity.Data;

/// <summary>
/// Abstraction for SQL dialect, e.g. syntax for different SQL server types and connection settings.
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    /// Gets a value indicating whether to automatically quote identifiers.
    /// Default is null, e.g. SqlSettings.AutoQuotedIdentifiers is used, which itself is false by default,
    /// but usually set to true in applications.
    /// </summary>
    public bool? AutoQuotedIdentifiers => null;

    /// <summary>
    /// Gets the comparer used to compare string field values for this connection,
    /// or null to fall back to <see cref="SqlSettings.DefaultComparer"/>. Dialects that
    /// support a per-connection comparer override this and <see cref="WithComparer"/>.
    /// </summary>
    public System.Collections.IComparer? Comparer => null;

    /// <summary>
    /// Returns a copy of this dialect with the specified comparer set. Dialects that
    /// support a per-connection comparer override this method; the default throws.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    /// <returns>The dialect with the comparer applied.</returns>
    public ISqlDialect WithComparer(System.Collections.IComparer? comparer) =>
        throw new NotSupportedException($"{GetType().Name} does not support a custom comparer.");

    /// <summary>
    /// Gets a value indicating whether the server supports the CONCAT() function
    /// (used by <c>ConcatAttribute</c> to decide between <c>CONCAT(...)</c>
    /// and an infix operator). This does not imply <see cref="ConcatOperator"/>
    /// is available: e.g. MySQL supports <c>CONCAT()</c> but has no infix
    /// concatenation operator (<c>||</c> is logical OR there unless
    /// PIPES_AS_CONCAT mode is on).
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
    /// <remarks>
    /// Dialects without an infix concatenation operator (e.g. MySQL, which
    /// concatenates via the <c>CONCAT()</c> function) throw
    /// <see cref="NotImplementedException"/>. Check <see cref="CanUseConcat"/>
    /// for <c>CONCAT()</c> function support instead; it is independent of this member.
    /// </remarks>
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
    /// Returns true if the specified identifier is a SQL keyword.
    /// </summary>
    /// <param name="keyword">The identifier to check.</param>
    /// <returns><c>true</c> if the identifier is a reserved SQL keyword; otherwise, <c>false</c>.</returns>
    bool IsReservedKeyword(string keyword)
    {
        return SqlSyntax.IsReservedKeywordForAny(keyword);
    }

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
    /// <remarks>
    /// Dialects without OFFSET support (check <see cref="CanUseOffsetFetch"/> first)
    /// throw <see cref="NotImplementedException"/>.
    /// </remarks>
    string OffsetFormat { get; }

    /// <summary>
    /// Gets the format for OFFSET FETCH statements.
    /// </summary>
    /// <value>
    /// The offset fetch format.
    /// </value>
    /// <remarks>
    /// Dialects without OFFSET support (check <see cref="CanUseOffsetFetch"/> first)
    /// throw <see cref="NotImplementedException"/>.
    /// </remarks>
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
    /// <returns>The quoted column alias.</returns>
    string QuoteColumnAlias(string s);

    /// <summary>
    /// Quotes the identifier, escaping embedded occurrences of the dialect's closing quote character.
    /// </summary>
    /// <param name="s">The identifier.</param>
    /// <returns>The quoted identifier.</returns>
    string QuoteIdentifier(string s);

    /// <summary>
    /// Quotes the unicode string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>The quoted unicode string.</returns>
    string QuoteUnicodeString(string s);

    /// <summary>
    /// Gets the SCOPE IDENTITY expression.
    /// </summary>
    /// <value>
    /// The SCOPE IDENTITY expression.
    /// </value>
    /// <remarks>
    /// Dialects that don't use SCOPE IDENTITY (check <see cref="UseScopeIdentity"/>
    /// first, e.g. dialects using RETURNING) throw
    /// <see cref="NotImplementedException"/>.
    /// </remarks>
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
    /// <remarks>
    /// Dialects without a SKIP keyword (check <see cref="CanUseSkipKeyword"/> first)
    /// throw <see cref="NotImplementedException"/>.
    /// </remarks>
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
    /// Gets the union keyword for the specified union type.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>The union keyword.</returns>
    /// <remarks>
    /// Throws <see cref="NotImplementedException"/> for union types the dialect
    /// doesn't support (e.g. INTERSECT / EXCEPT on MySQL and Firebird).
    /// </remarks>
    string UnionKeyword(SqlUnionType unionType);

    /// <summary>
    /// Gets a value indicating whether the server uses the datetime2 type.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the server uses datetime2; otherwise, <c>false</c>.
    /// </value>
    bool UseDateTime2 { get; }

    /// <summary>
    /// Gets a value indicating whether to use RETURNING identity.
    /// </summary>
    /// <value>
    ///   <c>true</c> if RETURNING identity should be used; otherwise, <c>false</c>.
    /// </value>
    bool UseReturningIdentity { get; }

    /// <summary>
    /// Gets a value indicating whether to use RETURNING INTO variable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if RETURNING INTO variable should be used; otherwise, <c>false</c>.
    /// </value>
    bool UseReturningIntoVar { get; }

    /// <summary>
    /// Gets a value indicating whether to use SCOPE IDENTITY.
    /// </summary>
    /// <value>
    ///   <c>true</c> if SCOPE IDENTITY should be used; otherwise, <c>false</c>.
    /// </value>
    bool UseScopeIdentity { get; }

    /// <summary>
    /// Gets a value indicating whether to use TAKE at the end.
    /// </summary>
    /// <value>
    ///   <c>true</c> if TAKE should be used at the end; otherwise, <c>false</c>.
    /// </value>
    bool UseTakeAtEnd { get; }

    /// <summary>
    /// Gets a value indicating whether the server supports ROWNUM.
    /// </summary>
    /// <value>
    ///   <c>true</c> if ROWNUM can be used; otherwise, <c>false</c>.
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
