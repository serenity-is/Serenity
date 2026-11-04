namespace Serenity.Extensions;

/// <summary>
/// Implemented by user rows that store the value calculated by
/// <see cref="BaseUserRetrieveService.NormalizeEmail"/> in a separate field. Users are
/// looked up by that field when they request a password reset, instead of comparing
/// the email by the collation of its column.
/// </summary>
/// <remarks>
/// The normalized field must be updated whenever the email changes, otherwise
/// the user can't be found by the new email.
/// </remarks>
public interface INormalizedEmailRow
{
    /// <summary>
    /// Gets the normalized email field.
    /// </summary>
    StringField NormalizedEmailField { get; }
}
