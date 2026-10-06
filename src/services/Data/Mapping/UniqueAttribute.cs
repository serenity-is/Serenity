namespace Serenity.Data.Mapping;

/// <summary>
/// Defines a unique constraint on the field. Sets the <see cref="FieldFlags.Unique"/> flag and,
/// by default, makes <see cref="UniqueFieldSaveBehavior"/> check the constraint before save by
/// running an existence query, so that a user friendly validation error is returned instead of a
/// database exception. The actual database unique index (if any) remains authoritative.
/// </summary>
/// <remarks>
/// Set <see cref="CheckBeforeSave"/> to <c>false</c> to skip the default pre-check, or
/// <see cref="DisableDefaultBehavior"/> to <c>true</c> to disable the default behavior entirely,
/// e.g. when you will replace it with your own behavior (derive from
/// <see cref="BaseUniqueConstraintSaveBehavior"/> or implement your own save behavior).
/// The default pre-check query may be unscoped; override
/// <see cref="BaseUniqueConstraintSaveBehavior.BuildUniqueConstraintQuery"/> to add row level
/// scoping (e.g. tenant/owner) so that it matches the actual database constraint.
/// </remarks>
/// <seealso cref="SetFieldFlagsAttribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class UniqueAttribute : SetFieldFlagsAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueAttribute"/> class.
    /// </summary>
    public UniqueAttribute()
        : base(FieldFlags.Unique)
    {
        CheckBeforeSave = true;
    }

    /// <summary>
    /// Gets or sets the name of the constraint. Not used.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to check this constraint before save.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should check, otherwise, <c>false</c>.
    /// </value>
    public bool CheckBeforeSave { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore deleted records.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should ignore deleted; otherwise, <c>false</c>.
    /// </value>
    public bool IgnoreDeleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore null values.
    /// </summary>
    /// <value>
    ///   <c>true</c> if should ignore value; otherwise, <c>false</c>.
    /// </value>
    public bool IgnoreNulls { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the default <see cref="UniqueFieldSaveBehavior"/>
    /// pre-check should be disabled for this field. Set to <c>true</c> if you want to replace the
    /// default behavior with your own.
    /// </summary>
    public bool DisableDefaultBehavior { get; set; }
}