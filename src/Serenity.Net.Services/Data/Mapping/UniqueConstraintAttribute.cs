namespace Serenity.Data.Mapping;

/// <summary>
/// Adds a unique constraint check to the row.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UniqueConstraintAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraintAttribute"/> class.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <exception cref="ArgumentNullException">fields</exception>
    public UniqueConstraintAttribute(params string[] fields)
    {
        if (fields.IsEmptyOrNull())
            throw new ArgumentNullException("fields");

        Fields = fields;
        CheckBeforeSave = true;
    }

    /// <summary>
    /// Gets or sets the constraint name. Not used at the moment.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

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
    public string ErrorMessage { get; set; }
}