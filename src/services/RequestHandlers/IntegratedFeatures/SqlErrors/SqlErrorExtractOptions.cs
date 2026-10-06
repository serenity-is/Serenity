namespace Serenity.Services.SqlErrors;

/// <summary>
/// Options for <see cref="ISqlErrorExtractor.Extract(Exception, SqlErrorExtractOptions?)"/>.
/// </summary>
public class SqlErrorExtractOptions
{
    /// <summary>
    /// Gets or sets the server type name, matching <see cref="ISqlDialect.ServerType"/>
    /// (e.g. <c>SqlServer</c>, <c>Postgres</c>). Typically taken from the current
    /// connection's dialect. When specified and recognized, it takes precedence over the
    /// type name based inference done by the extractor, which can fail for wrapped,
    /// mocked or custom provider exceptions.
    /// </summary>
    public string? ServerType { get; set; }
}
