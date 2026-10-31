using Microsoft.Extensions.FileSystemGlobbing;

namespace Serenity.Web;

/// <summary>
/// Decides which files of a static asset folder (e.g. a project's wwwroot) are served, using the same
/// include / exclude patterns that are used to select the files to publish / pack, so that a file that
/// would not be published is not served while running from the build output either.
/// Patterns are MSBuild style and relative to the project folder (e.g. wwwroot\**, wwwroot\**\*.tmp,
/// **\node_modules\**). They are converted to be relative to the asset folder: patterns that start with
/// the folder (wwwroot\) lose that prefix, patterns that start with **\ are kept, others can't match files
/// under the folder and are ignored.
/// </summary>
public sealed class StaticAssetPatternFilter
{
    private const string WellKnownFolder = ".well-known/";

    private readonly Matcher? includes;
    private readonly Matcher? excludes;
    private readonly Matcher? wellKnownExcludes;
    private readonly bool wellKnown;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="includes">Include patterns separated with ;, all files if empty</param>
    /// <param name="excludes">Exclude patterns separated with ;</param>
    /// <param name="wellKnown">True to include files under .well-known even if excludes match them, like
    /// the static web assets SDK does (it excludes dot folders but keeps .well-known)</param>
    /// <param name="wellKnownExcludes">Exclude patterns separated with ; for the .well-known folder</param>
    /// <param name="folder">Asset folder, relative to the project, that the patterns are rooted at</param>
    public StaticAssetPatternFilter(string? includes, string? excludes, bool wellKnown = false,
        string? wellKnownExcludes = null, string folder = "wwwroot")
    {
        var prefix = Normalize(folder).Trim('/') + "/";
        this.includes = CreateMatcher(includes, prefix);
        this.excludes = CreateMatcher(excludes, prefix);
        this.wellKnownExcludes = CreateMatcher(wellKnownExcludes, prefix);
        this.wellKnown = wellKnown;
    }

    /// <summary>
    /// Gets if the file should be served.
    /// </summary>
    /// <param name="path">Path relative to the asset folder, with / or \ separators</param>
    public bool IsMatch(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        path = Normalize(path).TrimStart('/');
        if (path.Length == 0)
            return false;

        if (wellKnown && path.StartsWith(WellKnownFolder, StringComparison.OrdinalIgnoreCase))
            return !Matches(wellKnownExcludes, path);

        if (includes != null && !Matches(includes, path))
            return false;

        return !Matches(excludes, path);
    }

    private static bool Matches(Matcher? matcher, string path)
    {
        return matcher != null && matcher.Match(path).HasMatches;
    }

    private static Matcher? CreateMatcher(string? patterns, string prefix)
    {
        Matcher? matcher = null;
        foreach (var pattern in (patterns ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var converted = ConvertPattern(pattern, prefix);
            if (converted == null)
                continue;

            matcher ??= new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude(converted);
        }

        return matcher;
    }

    private static string? ConvertPattern(string pattern, string prefix)
    {
        pattern = Normalize(pattern);
        if (pattern.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return pattern[prefix.Length..];

        if (pattern.StartsWith("**/", StringComparison.Ordinal))
            return pattern;

        return null;
    }

    private static string Normalize(string path)
    {
        return path.Replace('\\', '/');
    }
}
