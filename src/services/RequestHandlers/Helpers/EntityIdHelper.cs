using System.Globalization;

namespace Serenity.Services;

/// <summary>
/// Helper methods for converting a request's entity id to the row's id field type.
/// </summary>
internal static class EntityIdHelper
{
    /// <summary>
    /// Converts an entity id value to the type of the passed id field. If the value
    /// cannot be converted (e.g. a non-numeric value for an integer id), an
    /// invalid id <see cref="ValidationError"/> is thrown instead of the raw
    /// <see cref="FormatException"/>, <see cref="InvalidCastException"/> or
    /// <see cref="OverflowException"/>.
    /// </summary>
    /// <param name="idField">Id field to convert the value for.</param>
    /// <param name="entityId">Raw entity id value, usually from the request.</param>
    /// <param name="localizer">Text localizer.</param>
    /// <returns>The converted value, or <c>null</c> if <paramref name="entityId"/> is <c>null</c>.</returns>
    public static object? Convert(Field idField, object? entityId, ITextLocalizer? localizer)
    {
        try
        {
            return idField.ConvertValue(entityId, CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            throw DataValidation.InvalidIdError(idField, entityId, localizer);
        }
    }
}
