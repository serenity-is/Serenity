#if IsFeatureBuild || IsTemplateBuild
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Build;

/// <summary>
/// A fast glob implementation, compatible with .gitignore patterns of GIT
/// </summary>
public class GlobFilter
{
    private readonly GlobFilter excludeFilter;
    private readonly HashSet<string> extensions;
    private readonly List<string> startsWith;
    private readonly List<string> endsWith;
    private readonly List<string> contains;
    private readonly HashSet<string> exactMatch;
    private readonly List<Tuple<string, bool, string>> startsWithAndEndsWith;
    private readonly List<Tuple<string, bool, string>> containsAndEndsWith;

    private readonly bool isActive;
    private readonly List<Func<string, bool>> matchers;

    private readonly char[] DotAsteriskSlashBackQue = ['.', '*', '/', '\\', '?'];
    private readonly char[] AsteriskSlashBackQue = ['*', '/', '\\', '?'];
    private readonly char[] AsteriskQue = ['*', '?'];
    private readonly char[] FolderSeps = ['\\', '/'];

    /// <summary>
    /// Creates a new GlobFilter, containing both include and exclude patterns.
    /// When "include" is null / empty, all files are included by default, unless
    /// they match a pattern in "exclude" list.
    /// When "exclude" is null, it is ignored.
    /// </summary>
    /// <param name="include">List of include patterns</param>
    /// <param name="exclude">List of exclude patterns</param>
    public GlobFilter(IEnumerable<string> include, IEnumerable<string> exclude)
        : this(include)
    {
        if (exclude != null)
        {
            var excludes = exclude.Where(x => !string.IsNullOrEmpty(x));
            if (excludes.Any())
                excludeFilter = new GlobFilter(excludes);
        }
    }

    /// <summary>
    /// Creates a new GlobFilter, with just "include" globs.
    /// </summary>
    /// <param name="globs">List of patterns</param>
    public GlobFilter(IEnumerable<string> globs)
    {
        matchers = [];

        if (globs == null)
            return;

        foreach (var glob in globs)
        {
            if (string.IsNullOrEmpty(glob))
                continue;

            isActive = true;

            var sep = System.IO.Path.DirectorySeparatorChar;

            var s = glob;
            if (sep == '\\')
                s = glob.Replace('/', '\\');

            var starDotIndex = s.IndexOf("*.");

            // exact match
            if (s[0] == sep &&
                s[^1] != '/' &&
                s[^1] != '\\' &&
                s.IndexOfAny(AsteriskQue, 1) < 0)
            {
                exactMatch ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                exactMatch.Add(s[1..]);
                continue;
            }

            // simple extension filter (*.txt)
            if (starDotIndex == 0 &&
                s.IndexOfAny(DotAsteriskSlashBackQue, 2) < 0)
            {
                extensions ??= [];

                extensions.Add(s[1..]);
                continue;
            }

            // simple extension filter with extra dot (*.csproj.user)
            if (starDotIndex == 0 &&
                s.IndexOfAny(AsteriskSlashBackQue, 2) < 0)
            {
                endsWith ??= [];

                endsWith.Add(s[1..]);
                continue;
            }

            // subdirectory under root and extension (/bin/some/*.txt)
            if (starDotIndex > 1 &&
                s[starDotIndex - 1] == sep &&
                s[0] == sep &&
                s.IndexOfAny(AsteriskSlashBackQue, starDotIndex + 2) < 0 &&
                s.LastIndexOfAny(AsteriskQue, starDotIndex - 1) < 0)
            {
                startsWithAndEndsWith ??= [];

                startsWithAndEndsWith.Add(new Tuple<string, bool, string>(
                    s[1..starDotIndex], false, s[(starDotIndex + 1)..]));

                continue;
            }

            // extension under root (/*.txt)
            if (starDotIndex == 1 &&
                s[0] == sep &&
                s.IndexOfAny(AsteriskSlashBackQue, starDotIndex + 2) < 0)
            {
                startsWithAndEndsWith ??= [];

                startsWithAndEndsWith.Add(new Tuple<string, bool, string>(null, false, s[(starDotIndex + 1)..]));
                continue;
            }

            // directory filter at any depth (.git/)
            if (s[^1] == sep &&
                s[0] != sep &&
                s.IndexOfAny(AsteriskQue, 1) < 0)
            {
                contains ??= [];

                startsWith ??= [];

                contains.Add(sep.ToString() + s);
                startsWith.Add(s);

                continue;
            }

            // directory filter at root (/Imports/)
            if (s[^1] == sep &&
                s[0] == sep &&
                s.IndexOfAny(AsteriskQue) < 0)
            {
                startsWith ??= [];

                startsWith.Add(s[1..]);
                continue;
            }

            // folder than any then extension (App_Data/**/*.log) or (/App_Code/**/*.xyz)
            if (starDotIndex > 4 &&
                s[starDotIndex - 1] == sep &&
                s[starDotIndex - 2] == '*' &&
                s[starDotIndex - 3] == '*' &&
                s[starDotIndex - 4] == sep &&
                s.LastIndexOfAny(AsteriskQue, starDotIndex - 5) < 0 &&
                s.IndexOfAny(AsteriskSlashBackQue, starDotIndex + 2) < 0)
            {
                startsWithAndEndsWith ??= [];

                if (s[0] == sep)
                {
                    startsWithAndEndsWith.Add(new Tuple<string, bool, string>(
                        s.Substring(1, starDotIndex - 4), true, s[(starDotIndex + 1)..]));
                }
                else
                {
                    containsAndEndsWith ??= [];

                    startsWithAndEndsWith.Add(new Tuple<string, bool, string>(
                        s[..(starDotIndex - 3)], true, s[(starDotIndex + 1)..]));

                    containsAndEndsWith.Add(new Tuple<string, bool, string>(
                        sep.ToString() + s[..(starDotIndex - 3)], true, s[(starDotIndex + 1)..]));
                }

                continue;
            }

            if (s.IndexOfAny(AsteriskQue) < 0)
            {
                exactMatch ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (s[0] == sep)
                {
                    exactMatch.Add(s[..]);
                }
                else
                {
                    exactMatch.Add(s[..]);

                    endsWith ??= [];

                    endsWith.Add(sep + s[..]);
                }

                continue;
            }

            if (s[0] == '*' && s.IndexOfAny(AsteriskSlashBackQue, 1) < 0)
            {
                endsWith ??= [];

                endsWith.Add(s[1..]);
                continue;
            }

            if (s[0] == sep &&
                s.Length > 2 &&
                s[1] == '*' &&
                s.IndexOfAny(AsteriskSlashBackQue, 2) < 0)
            {
                startsWithAndEndsWith ??= [];

                startsWithAndEndsWith.Add(new Tuple<string, bool, string>(null, false, s[2..]));
                continue;
            }

            matchers.Add(ToMatcher(glob));
        }
    }

    private static Func<string, bool> ToMatcher(string glob)
    {
        return ToMatcherRegex(glob);
    }

    private static Func<string, bool> ToMatcherRegex(string glob)
    {
        var regex = WildcardToRegex(NormalizeGlob(glob));
        return regex.IsMatch;
    }

    /// <summary>
    /// Determines whether the specified path is matching this filter.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>
    ///   <c>true</c> if the specified path is match; otherwise, <c>false</c>.
    /// </returns>
    public bool IsMatch(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        if (excludeFilter != null &&
            excludeFilter.IsMatch(path))
            return false;

        if (!isActive)
            return true;

        if (extensions != null)
        {
            var extension = System.IO.Path.GetExtension(path);
            if (extension != null &&
                extensions.Contains(extension))
                return true;
        }

        if (contains != null &&
            contains.Any(x => path.Contains(x, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (exactMatch != null &&
            exactMatch.Contains(path))
            return true;

        if (startsWith != null &&
            startsWith.Any(x => path.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (endsWith != null &&
            endsWith.Any(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (startsWithAndEndsWith != null &&
            startsWithAndEndsWith.Any(x =>
            {
                if (!path.EndsWith(x.Item3, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (x.Item1 == null)
                {
                    if (path.Length > x.Item3.Length &&
                        path.LastIndexOfAny(FolderSeps, path.Length - x.Item3.Length - 1) >= 0)
                        return false;
                }
                else
                {
                    if (!path.StartsWith(x.Item1, StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (!x.Item2 &&
                        path.IndexOfAny(FolderSeps, x.Item1.Length) >= 0)
                    {
                        return false;
                    }
                }

                return true;
            }))
        {
            return true;
        }

        if (containsAndEndsWith != null &&
            containsAndEndsWith.Any(x =>
            {
                if (!path.EndsWith(x.Item3, StringComparison.OrdinalIgnoreCase))
                    return false;

                if (x.Item1 == null)
                {
                    if (path.Length > x.Item3.Length &&
                        path.LastIndexOfAny(FolderSeps, path.Length - x.Item3.Length - 1) >= 0)
                        return false;
                }
                else
                {
                    var idx = path.IndexOf(x.Item1, StringComparison.OrdinalIgnoreCase);
                    if (idx < 0)
                        return false;

                    if (!x.Item2 &&
                        path.IndexOfAny(FolderSeps, idx + x.Item1.Length) >= 0)
                    {
                        return false;
                    }
                }

                return true;
            }))
        {
            return true;
        }

        if (matchers != null && matchers.Any(x => x(path)))
            return true;

        return false;
    }

    /// <summary>
    /// Wildcards to regex conversion. Inspired from NuGet source code.
    /// </summary>
    /// <param name="wildcard">The wildcard.</param>
    /// <returns></returns>
    public static Regex WildcardToRegex(string wildcard)
    {
        var pattern = Regex.Escape(wildcard);
        if (System.IO.Path.DirectorySeparatorChar == '/')
        {
            // regex wildcard adjustments for *nix-style file systems
            pattern = pattern
                .Replace(@"\.\*\*", @"\.[^/.]*") // .** should not match on ../file or ./file but will match .file
                .Replace(@"\*\*/", "(.+/)*") //For recursive wildcards /**/, include the current directory.
                .Replace(@"\*\*", ".*") // For recursive wildcards that don't end in a slash e.g. **.txt would be treated as a .txt file at any depth
                .Replace(@"\*", @"[^/]*(/)?") // For non recursive searches, limit it any character that is not a directory separator
                .Replace(@"\?", "."); // ? translates to a single any character
        }
        else
        {
            // regex wildcard adjustments for Windows-style file systems
            pattern = pattern
                .Replace("/", @"\\") // On Windows, / is treated the same as \.
                .Replace(@"\.\*\*", @"\.[^\\.]*") // .** should not match on ../file or ./file but will match .file
                .Replace(@"\*\*\\", @"(.+\\)*") //For recursive wildcards \**\, include the current directory.
                .Replace(@"\*\*", ".*") // For recursive wildcards that don't end in a slash e.g. **.txt would be treated as a .txt file at any depth
                .Replace(@"\*", @"[^\\]*") // For non recursive searches, limit it any character that is not a directory separator
                .Replace(@"\?", "."); // ? translates to a single any character
        }

        return new Regex('^' + pattern + '$', RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
    }

    /// <summary>
    /// Normalizes the glob by replacing back slashes etc.
    /// </summary>
    /// <param name="glob">The glob.</param>
    /// <returns></returns>
    private static string NormalizeGlob(string glob)
    {
        if (string.IsNullOrEmpty(glob))
            return glob;

        if (glob.StartsWith('/') ||
            glob.StartsWith('\\'))
            glob = glob[1..];
        else if (!glob.StartsWith("**", StringComparison.Ordinal))
            glob = "**/" + glob;

        if (glob.EndsWith('/') ||
            glob.EndsWith('\\'))
            glob += "**/*";

        return glob;
    }
}
#endif