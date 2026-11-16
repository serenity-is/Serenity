namespace Serenity.ComponentModel;

/// <summary>
/// Why an object is not valid, see <see cref="ValidationRules.ValidateObject"/>: the first property whose value is not.
/// </summary>
/// <param name="propertyName">The name of the property.</param>
/// <param name="message">The message of the rule the value is not valid by, or null for a required value that is missing.</param>
public class ObjectValidationFailure(string propertyName, string? message)
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string PropertyName { get; } = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

    /// <summary>
    /// Gets the message of the rule the value is not valid by, or null when a required value is missing, which is told the
    /// way the required fields are, e.g. with the title of the field.
    /// </summary>
    public string? Message { get; } = message;

    /// <summary>
    /// Gets whether a required value is missing.
    /// </summary>
    public bool IsRequired => Message is null;
}
