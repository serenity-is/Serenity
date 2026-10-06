namespace Serenity.Data.Mapping;

/// <summary>
/// Adds a unique constraint check to the row. By default <see cref="UniqueConstraintSaveBehavior"/>
/// checks the constraint before save by running an existence query, so that a user friendly
/// validation error is returned instead of a database exception. The actual database unique
/// index (if any) remains authoritative.
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
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UniqueConstraintAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintAttribute"/> class.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <exception cref="ArgumentNullException">fields is null or empty.</exception>
    public UniqueConstraintAttribute(params string[] fields)
    {
        if (fields.IsEmptyOrNull())
            throw new ArgumentNullException(nameof(fields));

        Fields = fields;
        CheckBeforeSave = true;
    }

    /// <summary>
    /// Gets or sets the constraint name. Not used at the moment.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>
    /// The fields.
    /// </value>
    public string[] Fields { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether constraint should be checked before save, default true.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [check before save]; otherwise, <c>false</c>.
    /// </value>
    public bool CheckBeforeSave { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore deleted records while checking the constraint.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [ignore deleted]; otherwise, <c>false</c>.
    /// </value>
    public bool IgnoreDeleted { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the default <see cref="UniqueConstraintSaveBehavior"/>
    /// pre-check should be disabled for this constraint. Set to <c>true</c> if you want to replace
    /// the default behavior with your own.
    /// </summary>
    public bool DisableDefaultBehavior { get; set; }
}