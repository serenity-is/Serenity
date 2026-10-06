namespace Serenity.Services.SqlErrors;

/// <summary>
/// Database operation that caused a constraint violation.
/// </summary>
public enum SqlErrorOperation
{
    /// <summary>
    /// An insert operation.
    /// </summary>
    Insert,

    /// <summary>
    /// An update operation.
    /// </summary>
    Update,

    /// <summary>
    /// A delete operation.
    /// </summary>
    Delete,

    /// <summary>
    /// An undelete operation.
    /// </summary>
    Undelete
}
