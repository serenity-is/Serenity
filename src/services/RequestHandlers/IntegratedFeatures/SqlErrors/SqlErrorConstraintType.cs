namespace Serenity.Services.SqlErrors;

/// <summary>
/// Type of a database constraint violation.
/// </summary>
public enum SqlErrorConstraintType
{
    /// <summary>
    /// A primary key violation.
    /// </summary>
    PrimaryKey,

    /// <summary>
    /// A unique constraint or unique index violation.
    /// </summary>
    Unique,

    /// <summary>
    /// A foreign key violation.
    /// </summary>
    ForeignKey,

    /// <summary>
    /// A not null constraint violation.
    /// </summary>
    NotNull
}
