using Serenity.TypeScript.TsTypes;
using System.Diagnostics;

namespace Serenity.TypeScript.TsParser;

partial class Scanner
{
    internal static int SkipTrivia(string text, int pos, bool stopAfterLineBreak = false,
        bool stopAtComments = false, bool inJSDoc = false)
    {
        if (pos < 0)
            return pos;

        var canConsumeStar = false;
        while (true)
        {
            if (pos >= text.Length)
                return pos;

            var ch = text[pos];
            switch (ch)
            {
                case '\r':
                    if (pos + 1 < text.Length && text[pos + 1] == '\n')
                        pos++;
                    goto newLineCase;

                case '\n':
                newLineCase:
                    pos++;
                    if (stopAfterLineBreak)
                        return pos;

                    canConsumeStar = !!inJSDoc;
                    continue;

                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    pos++;
                    continue;

                case '/':
                    if (stopAtComments)
                        break;

                    if (pos + 1 < text.Length && text[pos + 1] == '/')
                    {
                        pos += 2;
                        while (pos < text.Length)
                        {
                            if (IsLineBreak(text[pos]))
                                break;

                            pos++;
                        }
                        canConsumeStar = false;
                        continue;
                    }

                    if (pos < text.Length && text[pos + 1] == '*')
                    {
                        pos += 2;
                        while (pos < text.Length)
                        {
                            if (text[pos] == '*' && pos + 1 < text.Length && text[pos + 1] == '/')
                            {
                                pos += 2;
                                break;
                            }

                            pos++;
                        }
                        canConsumeStar = false;
                        continue;
                    }
                    break;

                case '<':
                case '|':
                case '=':
                case '>':
                    if (IsConflictMarkerTrivia(text, pos))
                    {
                        pos = ScanConflictMarkerTrivia(text, pos);
                        canConsumeStar = false;
                        continue;
                    }
                    break;

                case '#':
                    if (pos == 0 && IsShebangTrivia(text, pos))
                    {
                        pos = ScanShebangTrivia(text, pos);
                        canConsumeStar = false;
                        continue;
                    }
                    break;

                case '*':
                    if (canConsumeStar)
                    {
                        pos++;
                        canConsumeStar = false;
                        continue;
                    }
                    break;

                default:
                    if (ch > CharacterCodes.MaxAsciiCharacter && IsWhiteSpaceLike(ch))
                    {
                        pos++;
                        continue;
                    }
                    break;
            }
            return pos;
        }
    }

    // All conflict markers consist of the same character repeated seven times.  If it is
    // a <<<<<<< or >>>>>>> marker then it is also followed by a space.
    private static readonly int mergeConflictMarkerLength = "<<<<<<<".Length;

    public static bool IsConflictMarkerTrivia(string text, int pos)
    {
        Debug.Assert(pos >= 0);

        // Conflict markers must be at the start of a line.
        if (pos == 0 || (pos - 1 < text.Length && IsLineBreak(text[pos - 1])))
        {
            if ((pos + mergeConflictMarkerLength) < text.Length)
            {
                var ch = text[pos];
                for (var i = 0; i < mergeConflictMarkerLength; i++)
                {
                    if (text[pos + i] != ch)
                        return false;
                }

                return ch == '=' || text[pos + mergeConflictMarkerLength] == ' ';
            }
        }
        return false;
    }

    public static int ScanConflictMarkerTrivia(string text, int pos, Action<DiagnosticMessage, int> error = null)
    {
        error?.Invoke(DiagnosticMessage.Merge_conflict_marker_encountered, mergeConflictMarkerLength);

        var len = text.Length;
        if (pos >= len)
            return pos;

        var ch = text[pos];
        if (ch == '<' || ch == '>')
        {
            while (pos < len && !IsLineBreak(text[pos]))
                pos++;
        }
        else
        {
            Debug.Assert(ch == '|' || ch == '=');
            while (pos < len)
            {
                var currentChar = text[pos];
                if ((currentChar == '=' || currentChar == '>') &&
                    currentChar != ch && IsConflictMarkerTrivia(text, pos))
                    break;
                pos++;
            }
        }

        return pos;
    }

#if ISSOURCEGENERATOR
    private static readonly Regex shebangTriviaRegex = new("/^#!.*/", RegexOptions.Compiled);
#else
    private static readonly Regex shebangTriviaRegex = shebangTriviaRegexGen();

    [GeneratedRegex("/^#!.*/")]
    private static partial Regex shebangTriviaRegexGen();
#endif

    /** Optionally, get the shebang */
    private static string GetShebang(string text)
    {
        var match = shebangTriviaRegex.Match(text);
        if (match.Success && match.Captures.Count > 0)
            return match.Captures[0].Value;
        return null;
    }

    private static bool IsShebangTrivia(string text, int pos)
    {
        // Shebangs check must only be done at the start of the file
        Debug.Assert(pos == 0);
        return shebangTriviaRegex.IsMatch(text);
    }

    public static int ScanShebangTrivia(string text, int pos)
    {
        var shebang = shebangTriviaRegex.Match(text);
        if (shebang.Success && shebang.Captures.Count > 0)
            pos += shebang.Captures[0].Length;
        return pos;
    }

}