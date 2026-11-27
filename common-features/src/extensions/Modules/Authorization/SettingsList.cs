namespace Serenity.Extensions;

/// <summary>
/// Splits a list that a setting holds as a single text, e.g. "admin, ops", separated by commas, semicolons or white space.
/// A text, unlike an array, replaces the value of a setting as a whole in every settings file and environment variable,
/// where the items of an array would be merged by their index.
/// </summary>
public static class SettingsList
{
    private static readonly char[] Separators = [',', ';', ' ', '\t', '\r', '\n'];

    /// <summary>
    /// Splits a list into its items, without empty ones.
    /// </summary>
    /// <param name="value">The list, e.g. "admin, ops".</param>
    /// <returns>The items, e.g. ["admin", "ops"], or none for an empty or null list.</returns>
    public static string[] Split(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        return value.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
