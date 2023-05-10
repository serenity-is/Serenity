namespace Serenity.Data.Mapping;

/// <summary>
/// Defines a unique constraint on the field
/// </summary>
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
    public string Name { get; set; }

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
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string ErrorMessage { get; set; }
}