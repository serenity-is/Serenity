namespace Serenity.Services.SqlErrors;

/// <summary>
/// Extracts constraint violation information from a database exception.
/// Implementations may be registered as a singleton to customize how
/// primary key, unique, foreign key and not null violations are recognized
/// and described for a specific database provider.
/// </summary>
public interface ISqlErrorExtractor
{
    /// <summary>
    /// Extracts constraint violation information from the given exception,
    /// or returns <c>null</c> if it is not a recognized constraint violation.
    /// </summary>
    /// <param name="exception">Exception that occurred during a database operation.</param>
    /// <param name="options">Optional extraction options, e.g. the server type taken
    /// from the current connection's dialect, which takes precedence over type name
    /// based inference.</param>
    SqlErrorInfo? Extract(Exception exception, SqlErrorExtractOptions? options = null);
}
