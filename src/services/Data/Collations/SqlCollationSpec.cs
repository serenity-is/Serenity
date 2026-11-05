namespace Serenity.Data;

/// <summary>
/// An engine independent description of the comparison a collation should perform.
/// </summary>
/// <param name="Culture">The culture name, an empty string for the invariant culture, or null for
/// an ordinal comparison.</param>
/// <param name="IgnoreCase">True to compare case insensitively.</param>
/// <param name="IgnoreAccents">True to compare accent insensitively.</param>
public readonly record struct SqlCollationSpec(string? Culture, bool IgnoreCase = false, bool IgnoreAccents = false)
{
    /// <summary>
    /// Tries to parse a comparer specification as accepted by <see cref="ComparerResolver"/>, e.g.
    /// "Ordinal", "OrdinalIgnoreCase", "InvariantCulture" or "tr-TR:IgnoreCase:IgnoreAccents".
    /// </summary>
    /// <remarks>
    /// CurrentCulture and CurrentCultureIgnoreCase are not accepted. A collation is stored
    /// in the database, so it can't depend on the culture of the process that creates it.
    /// </remarks>
    /// <param name="value">The comparer specification.</param>
    /// <param name="spec">The parsed value, if successful.</param>
    /// <returns>True if the specification can be mapped to a collation.</returns>
    public static bool TryParse(string? value, out SqlCollationSpec spec)
    {
        spec = default;
        value = value.TrimToNull();
        if (value is null)
            return false;

        if (Enum.TryParse<StringComparison>(value, ignoreCase: true, out var comparison) &&
            Enum.IsDefined(comparison))
        {
            switch (comparison)
            {
                case StringComparison.Ordinal:
                    spec = new(null);
                    return true;
                case StringComparison.OrdinalIgnoreCase:
                    spec = new(null, IgnoreCase: true);
                    return true;
                case StringComparison.InvariantCulture:
                    spec = new("");
                    return true;
                case StringComparison.InvariantCultureIgnoreCase:
                    spec = new("", IgnoreCase: true);
                    return true;
                default:
                    return false;
            }
        }

        var parts = value.Split(':', StringSplitOptions.TrimEntries);
        var ignoreCase = false;
        var ignoreAccents = false;
        foreach (var option in parts.Skip(1))
        {
            if (option.Equals("IgnoreCase", StringComparison.OrdinalIgnoreCase))
                ignoreCase = true;
            else if (option.Equals("IgnoreAccents", StringComparison.OrdinalIgnoreCase))
                ignoreAccents = true;
            else
                return false;
        }

        try
        {
            spec = new(CultureInfo.GetCultureInfo(parts[0]).Name, ignoreCase, ignoreAccents);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var culture = Culture is null ? "Ordinal" : Culture.Length == 0 ? "Invariant" : Culture;
        return culture + (IgnoreCase ? ":IgnoreCase" : "") + (IgnoreAccents ? ":IgnoreAccents" : "");
    }
}
