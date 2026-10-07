namespace Serenity.Data;

/// <summary>
/// Parses comparer specifications like "Ordinal", "OrdinalIgnoreCase", "CurrentCulture",
/// "CurrentCultureIgnoreCase", "tr-TR" or "tr-TR:IgnoreCase" into a <see cref="StringComparer"/>.
/// </summary>
public static class ComparerResolver
{
    /// <summary>
    /// Parses the specified comparer specification. Returns null for null or empty values.
    /// </summary>
    /// <param name="value">The comparer specification.</param>
    /// <returns>The parsed string comparer, or null if <paramref name="value"/> is null or empty.</returns>
    /// <exception cref="ArgumentException">The comparer specification is not valid.</exception>
    public static StringComparer? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Trim();

        if (Enum.TryParse<StringComparison>(value, ignoreCase: true, out var comparison) &&
            Enum.IsDefined(comparison))
            return StringComparer.FromComparison(comparison);

        var parts = value.Split(':', 2);
        var cultureName = parts[0].Trim();
        var ignoreCase = false;

        if (parts.Length > 1)
        {
            if (!parts[1].Trim().Equals("IgnoreCase", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    $"Invalid comparer specification '{value}'. Expected a StringComparison name " +
                    "or 'culture' / 'culture:IgnoreCase'.", nameof(value));

            ignoreCase = true;
        }

        try
        {
            return StringComparer.Create(CultureInfo.GetCultureInfo(cultureName), ignoreCase);
        }
        catch (CultureNotFoundException)
        {
            throw new ArgumentException(
                $"Invalid comparer specification '{value}'. Culture '{cultureName}' was not found.",
                nameof(value));
        }
    }

    /// <summary>
    /// Tries to parse the specified comparer specification.
    /// </summary>
    /// <param name="value">The comparer specification.</param>
    /// <param name="comparer">The parsed string comparer, if successful.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public static bool TryParse(string? value,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out StringComparer? comparer)
    {
        comparer = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            comparer = Parse(value);
            return comparer is not null;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
