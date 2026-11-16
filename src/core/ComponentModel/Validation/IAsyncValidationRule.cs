using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// A validation attribute that can also be checked asynchronously, e.g. by a query to the database, which the asynchronous
/// save handlers await instead of calling <see cref="ValidationAttribute.GetValidationResult"/>. It still has to be valid to
/// check synchronously, as the synchronous save handlers and other validators, like the ones of ASP.NET, only do that.
/// <see cref="ValidationRuleAttribute"/> implements it, see its <c>IsValidValueAsync</c>.
/// </summary>
public interface IAsyncValidationRule
{
    /// <summary>
    /// Checks a value asynchronously.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="context">The context, e.g. for its services, like the connection of a save handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="ValidationResult.Success"/>, i.e. null, when the value is valid, or the result with the message.</returns>
    ValueTask<ValidationResult?> IsValidAsync(object? value, ValidationContext context, CancellationToken cancellationToken = default);
}
