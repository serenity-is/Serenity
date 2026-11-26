namespace Serenity.Web;

/// <summary>
/// A glob pattern for <see cref="EsmAccessControlOptions"/>, which matches a path relative to an <c>esm</c>
/// folder, or the full request path if it starts with a slash. Supports <c>**</c> (any characters),
/// <c>*</c> (any characters except a slash) and <c>?</c> (a single character except a slash).
/// Matching is case-insensitive.
/// </summary>
public class EsmPathPattern
{
    private readonly Regex regex;

    /// <summary>
    /// Creates a new pattern.
    /// </summary>
    /// <param name="pattern">The glob pattern</param>
    public EsmPathPattern(string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        Pattern = pattern.Trim();
        IsFullPath = Pattern.StartsWith('/');

        var sb = new StringBuilder("^");
        for (var i = 0; i < Pattern.Length; i++)
        {
            var c = Pattern[i];
            if (c == '*')
            {
                if (i + 1 < Pattern.Length && Pattern[i + 1] == '*')
                {
                    i++;
                    // "**/" also matches no folders at all
                    if (i + 1 < Pattern.Length && Pattern[i + 1] == '/')
                    {
                        i++;
                        sb.Append("(?:.*/)?");
                    }
                    else
                        sb.Append(".*");
                }
                else
                    sb.Append("[^/]*");
            }
            else if (c == '?')
                sb.Append("[^/]");
            else
                sb.Append(Regex.Escape(c.ToString()));
        }
        sb.Append('$');

        regex = new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    /// <summary>
    /// The glob pattern.
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// True if the pattern matches the full request path, e.g. it starts with a slash.
    /// </summary>
    public bool IsFullPath { get; }

    /// <summary>
    /// Returns true if the pattern matches the file.
    /// </summary>
    /// <param name="relativePath">The path relative to the <c>esm</c> folder</param>
    /// <param name="fullPath">The full request path, excluding the path base</param>
    public bool IsMatch(string relativePath, string fullPath)
    {
        return regex.IsMatch(IsFullPath ? fullPath : relativePath);
    }

    /// <summary>
    /// Parses semicolon separated patterns.
    /// </summary>
    /// <param name="patterns">The patterns</param>
    public static List<EsmPathPattern> ParseList(string? patterns)
    {
        if (string.IsNullOrWhiteSpace(patterns))
            return [];

        return [.. patterns.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => new EsmPathPattern(x))];
    }
}
