using System.Runtime.CompilerServices;

namespace Serenity.Data;

/// <summary>
/// Shared SQL skip-region scanner: quoted strings / quoted identifiers
/// (' " `) and -- / /* */ comments.
/// </summary>
/// <remarks>
/// Rules: comment markers only apply outside quotes, quotes don't span comments,
/// block comments don't nest, only '\n' ends a line comment, and '' needs no
/// special case (close+reopen is parity-neutral for toggle scanners). An unclosed
/// quote or block comment consumes the rest of the string.
/// </remarks>
internal static class SqlRegionScanner
{
    /// <summary>
    /// Tries to consume a quoted region or comment starting at index.
    /// </summary>
    /// <param name="expression">The SQL expression.</param>
    /// <param name="index">The index. On success, advanced to the region's last character.</param>
    /// <returns><c>true</c> if a region started at index and was consumed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TrySkipQuotesAndComments(string expression, ref int index)
    {
        var c = expression[index];

        if (c == '\'' || c == '"' || c == '`')
        {
            var end = expression.IndexOf(c, index + 1);
            index = end < 0 ? expression.Length - 1 : end;
            return true;
        }

        if (c == '-' && index + 1 < expression.Length && expression[index + 1] == '-')
        {
            var end = expression.IndexOf('\n', index + 2);
            index = end < 0 ? expression.Length - 1 : end;
            return true;
        }

        if (c == '/' && index + 1 < expression.Length && expression[index + 1] == '*')
        {
            var end = expression.IndexOf("*/", index + 2, StringComparison.Ordinal);
            index = end < 0 ? expression.Length - 1 : end + 1;
            return true;
        }

        return false;
    }
}
