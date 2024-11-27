using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace RootProjectWizard
{
    /// <summary>
    /// Inspired from NuGet source code
    /// </summary>
    public class PathMatcher
    {
        private readonly IEnumerable<Regex> includes;
        private readonly IEnumerable<Regex> excludes;

        public PathMatcher(IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            this.includes = includes.Select(x => WildcardToRegex(x));
            this.excludes = excludes.Select(x => WildcardToRegex(x));
        }

        public bool IsMatch(string path)
        {
            if (this.excludes.Any(x => x.IsMatch(path)))
                return false;

            return this.includes.Any(x => x.IsMatch(path));
        }

        private static Regex WildcardToRegex(string wildcard)
        {
            var pattern = Regex.Escape(wildcard);
            if (Path.DirectorySeparatorChar == '/')
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
                    .Replace(@"\*", @"[^\\]*(\\)?") // For non recursive searches, limit it any character that is not a directory separator
                    .Replace(@"\?", "."); // ? translates to a single any character
            }

            return new Regex('^' + pattern + '$', RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }
    }
}
