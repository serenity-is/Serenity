namespace Serenity.Data;

/// <summary>
/// Specifies the comparer used to compare the values of a row's fields (for example, for
/// change detection or dirty tracking). Can be applied to a row class to affect all of its
/// fields, or to a single field property to override the row, connection or global setting.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ComparerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance using a <see cref="StringComparison"/> value.
    /// </summary>
    /// <param name="comparison">The string comparison.</param>
    public ComparerAttribute(StringComparison comparison)
    {
        Comparison = comparison;
    }

    /// <summary>
    /// Initializes a new instance using a culture name.
    /// </summary>
    /// <param name="culture">The culture name, e.g. "tr-TR".</param>
    public ComparerAttribute(string culture)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        CaseInsensitive = false;
    }

    /// <summary>
    /// Initializes a new instance using a culture name and a case-insensitive flag.
    /// </summary>
    /// <param name="culture">The culture name, e.g. "tr-TR".</param>
    /// <param name="caseInsensitive">True to compare case-insensitively.</param>
    public ComparerAttribute(string culture, bool caseInsensitive)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        CaseInsensitive = caseInsensitive;
    }

    /// <summary>
    /// Gets the string comparison, if specified.
    /// </summary>
    public StringComparison? Comparison { get; }

    /// <summary>
    /// Gets the culture name, if specified.
    /// </summary>
    public string? Culture { get; }

    /// <summary>
    /// Gets a value indicating whether to compare case-insensitively.
    /// </summary>
    public bool? CaseInsensitive { get; }

    /// <summary>
    /// Creates the <see cref="StringComparer"/> represented by this attribute.
    /// </summary>
    /// <returns>The string comparer.</returns>
    public StringComparer ToStringComparer()
    {
        if (Comparison is StringComparison comparison)
            return StringComparer.FromComparison(comparison);

        if (Culture is not null)
            return StringComparer.Create(CultureInfo.GetCultureInfo(Culture), CaseInsensitive ?? false);

        return CaseInsensitive == true ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
    }
}
