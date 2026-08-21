
namespace Serenity;

/// <summary>
/// Custom validator abstraction.
/// </summary>
public interface ICustomValidator
{
    /// <summary>
    /// Validates the value using the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The validation error message, or null if the value is valid.</returns>
    string Validate(IValidationContext context);
}