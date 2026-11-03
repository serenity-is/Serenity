namespace Serenity.Extensions;

/// <summary>
/// Combines a regular expression that adds to a default one, so that configuring a pattern can't lose what the default
/// covers by accident, like sensitive tables. Leaving out the default takes setting an option for that.
/// </summary>
public static class RegexWithDefault
{
    /// <summary>
    /// Gets the regular expression matching what the default or the configured pattern matches.
    /// </summary>
    /// <param name="useDefault">Whether the default pattern is used.</param>
    /// <param name="defaultPattern">The default pattern.</param>
    /// <param name="pattern">The configured pattern, if any.</param>
    /// <returns>The combined pattern, or null when nothing is to be matched.</returns>
    public static string? Combine(bool useDefault, string defaultPattern, string? pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return useDefault ? defaultPattern : null;

        // the groups of the default don't capture, not to change what the numbered groups in the pattern refer to
        return useDefault ? $"(?n:{defaultPattern})|(?:{pattern})" : pattern;
    }
}
