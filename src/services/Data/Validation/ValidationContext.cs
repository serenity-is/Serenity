namespace Serenity;

/// <summary>
/// Validation context abstraction
/// </summary>
public interface IValidationContext
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    object Value { get; }

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns></returns>
    object GetFieldValue(string fieldName);

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    IDbConnection Connection { get; }

    /// <summary>
    /// Localizer
    /// </summary>
    ITextLocalizer Localizer { get; }
}