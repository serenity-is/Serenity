using System.Text.Json;

namespace Serenity.Data;

/// <summary>
/// Helper methods for validating base64 encoded field values.
/// </summary>
internal static class Base64Helper
{
    /// <summary>
    /// Gets the maximum base64 encoded length allowed for a field of the given
    /// size, or <see cref="int.MaxValue"/> when the size is not specified.
    /// </summary>
    /// <param name="size">The maximum size in bytes.</param>
    /// <returns>The maximum base64 encoded length.</returns>
    public static int GetMaxLength(int size)
    {
        return size > 0 ? 4 * ((size + 2) / 3) : int.MaxValue;
    }

    /// <summary>
    /// Gets a message for a base64 value that exceeds the maximum field size.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>The error message.</returns>
    public static string GetTooLongMessage(Field field)
    {
        return string.Format("Base64 value for field '{0}' exceeds the maximum size of {1} bytes!",
            field.Name, field.Size);
    }

    /// <summary>
    /// Counts the base64 characters (ignoring whitespace) in a value.
    /// </summary>
    /// <param name="value">The base64 value.</param>
    /// <returns>The number of non-whitespace characters.</returns>
    public static int CountCharacters(ReadOnlySpan<char> value)
    {
        var count = 0;
        foreach (var c in value)
            if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                count++;

        return count;
    }

    /// <summary>
    /// Counts the base64 characters (ignoring whitespace) in a UTF-8 value.
    /// </summary>
    /// <param name="value">The base64 value.</param>
    /// <returns>The number of non-whitespace characters.</returns>
    public static int CountCharacters(ReadOnlySpan<byte> value)
    {
        var count = 0;
        foreach (var b in value)
            if (b != (byte)' ' && b != (byte)'\t' && b != (byte)'\r' && b != (byte)'\n')
                count++;

        return count;
    }

    /// <summary>
    /// Counts the base64 characters (ignoring whitespace) in the current JSON string token.
    /// </summary>
    /// <param name="reader">The reader positioned on a string token.</param>
    /// <returns>The number of non-whitespace characters.</returns>
    public static int GetLength(ref Utf8JsonReader reader)
    {
        if (reader.HasValueSequence)
        {
            var count = 0;
            foreach (var segment in reader.ValueSequence)
                count += CountCharacters(segment.Span);

            return count;
        }

        return CountCharacters(reader.ValueSpan);
    }
}
