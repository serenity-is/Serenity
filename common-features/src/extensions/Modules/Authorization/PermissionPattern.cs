namespace Serenity.Extensions;

/// <summary>
/// Matches a permission key against a pattern, where "*" stands for any text, e.g. "Northwind:*" or "*:Delete", ignoring
/// case.
/// </summary>
public static class PermissionPattern
{
    /// <summary>
    /// Checks whether a permission key matches a pattern.
    /// </summary>
    /// <param name="pattern">The pattern, e.g. "Administration:*".</param>
    /// <param name="permissionKey">The permission key, e.g. "Administration:Security".</param>
    /// <returns>True when the key matches the pattern.</returns>
    public static bool IsMatch(string? pattern, string? permissionKey)
    {
        if (pattern == null || permissionKey == null)
            return false;

        int p = 0, k = 0, star = -1, mark = 0;
        while (k < permissionKey.Length)
        {
            if (p < pattern.Length && pattern[p] != '*' &&
                char.ToUpperInvariant(pattern[p]) == char.ToUpperInvariant(permissionKey[k]))
            {
                p++;
                k++;
            }
            else if (p < pattern.Length && pattern[p] == '*')
            {
                // a star matches nothing at first, and one more character each time the rest does not match
                star = p++;
                mark = k;
            }
            else if (star >= 0)
            {
                p = star + 1;
                k = ++mark;
            }
            else
                return false;
        }

        while (p < pattern.Length && pattern[p] == '*')
            p++;

        return p == pattern.Length;
    }
}
