
namespace Serenity;

/// <summary>
/// Custom validator abstraction
/// </summary>
public interface ICustomValidator
{
    /// <summary>
    /// Validates value using specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    string Validate(IValidationContext context);
}