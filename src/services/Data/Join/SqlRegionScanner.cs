using System.Runtime.CompilerServices;

namespace Serenity.Data;

/// <summary>
/// Shared SQL skip-region scanner: quoted strings / quoted identifiers
/// (' " ` and [...]) and -- / /* */ comments.
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
    /// <param name="skipBracketIdentifiers">Whether bracket-quoted identifiers should be skipped.</param>
    /// <returns><c>true</c> if a region started at index and was consumed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TrySkipQuotesAndComments(string expression, ref int index, bool skipBracketIdentifiers = true)
    {
        var c = expression[index];

        if (skipBracketIdentifiers && c == '[')
            return TryReadBracketedIdentifier(expression, ref index, out _);

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

    /// <summary>
    /// Reads a bracket-quoted identifier starting at index. Escaped closing
    /// brackets are represented as <c>]]</c>.
    /// </summary>
    /// <param name="expression">The SQL expression.</param>
    /// <param name="index">The index. On success, advanced to the closing bracket or end of input.</param>
    /// <param name="identifier">The unquoted identifier, or null for an unclosed identifier.</param>
    /// <returns><c>true</c> if a bracket-quoted identifier started at index.</returns>
    internal static bool TryReadBracketedIdentifier(string expression, ref int index, out string? identifier)
    {
        identifier = null;
        if (expression[index] != '[')
            return false;

        var start = index + 1;
        var i = start;
        StringBuilder? unescaped = null;

        while (i < expression.Length)
        {
            if (expression[i] != ']')
            {
                i++;
                continue;
            }

            if (i + 1 < expression.Length && expression[i + 1] == ']')
            {
                unescaped ??= new StringBuilder();
                unescaped.Append(expression, start, i - start).Append(']');
                i += 2;
                start = i;
                continue;
            }

            identifier = unescaped is null ? expression[start..i] : unescaped.Append(expression, start, i - start).ToString();
            index = i;
            return true;
        }

        index = expression.Length - 1;
        return true;
    }
}
