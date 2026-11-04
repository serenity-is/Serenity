namespace Serenity.Extensions;

/// <summary>
/// Implemented by user rows that store the value calculated by
/// <see cref="BaseUserRetrieveService.NormalizeUsername"/> in a separate field.
/// <see cref="BaseUserRetrieveService{TRow}"/> looks these users up by that field,
/// instead of comparing the username by the collation of its column.
/// </summary>
public interface INormalizedUsernameRow
{
    /// <summary>
    /// Gets the normalized username field.
    /// </summary>
    StringField NormalizedUsernameField { get; }
}
