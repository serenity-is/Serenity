namespace Serenity.Data;

/// <summary>
/// Expected number of rows enumeration for SQL Update / Delete operation.
/// This enumeration is used to avoid updating/deleting multiple records
/// by mistake when forgot to add a WHERE statement.
/// </summary>
public enum ExpectedRows
{
    /// <summary>
    /// Query should only affect One row, not zero or more
    /// </summary>
    One = 0,
    /// <summary>
    /// Query may affect zero or one row, not more
    /// </summary>
    ZeroOrOne = 1,
    /// <summary>
    /// Ignore number of affected rows
    /// </summary>
    Ignore = 2,
}