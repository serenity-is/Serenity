using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.TsParser;

partial class Scanner
{
    private static U IterateCommentRanges<T, U>(bool reduce, string text, int pos, bool trailing, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial = default)
    {
        int pendingPos = 0;
        int pendingEnd = 0;
        var pendingKind = SyntaxKind.Unknown;
        bool pendingHasTrailingNewLine = false;
        var hasPendingCommentRange = false;
        var collecting = trailing;
        var accumulator = initial;
        if (pos == 0)
        {
            collecting = true;
            var shebang = GetShebang(text);
            if (shebang != null)
                pos = shebang.Length;
        }
        while (pos >= 0 && pos < text.Length)
        {
            var ch = text[pos];
            switch (ch)
            {
                case '\r':
                    if (pos + 1 < text.Length && text[pos + 1] == '\n')
                        pos++;
                    goto lineFeedLabel;
                case '\n':
                lineFeedLabel:
                    pos++;
                    if (trailing)
                        goto breakScan;

                    collecting = true;
                    if (hasPendingCommentRange)
                        pendingHasTrailingNewLine = true;
                    continue;

                case '\t':
                case '\v':
                case '\f':
                case ' ':
                    pos++;
                    continue;

                case '/':
                    char nextChar;
                    var hasTrailingNewLine = false;
                    if (pos + 1 < text.Length && ((nextChar = text[pos + 1]) == '/' || nextChar == '*'))
                    {
                        var kind = nextChar == '/' ? SyntaxKind.SingleLineCommentTrivia : SyntaxKind.MultiLineCommentTrivia;
                        var startPos = pos;
                        pos += 2;
                        if (nextChar == '/')
                        {
                            while (pos < text.Length)
                            {
                                if (IsLineBreak(text[pos]))
                                {
                                    hasTrailingNewLine = true;
                                    break;
                                }
                                pos++;
                            }
                        }
                        else
                        {
                            while (pos < text.Length)
                            {
                                if (text[pos] == '*' && pos + 1 < text.Length && text[pos + 1] == '/')
                                {
                                    pos += 2;
                                    break;
                                }
                                pos++;
                            }
                        }
                        if (collecting)
                        {
                            if (hasPendingCommentRange)
                            {
                                accumulator = cb((pendingPos, pendingEnd, pendingKind, pendingHasTrailingNewLine, state, accumulator));
                                if (!reduce && accumulator != null)
                                {
                                    // If we are not reducing and we have a truthy result, return it.
                                    return accumulator;
                                }
                            }
                            pendingPos = startPos;
                            pendingEnd = pos;
                            pendingKind = kind;
                            pendingHasTrailingNewLine = hasTrailingNewLine;
                            hasPendingCommentRange = true;
                        }
                        continue;
                    }
                    goto breakScan;

                default:
                    if (ch > CharacterCodes.MaxAsciiCharacter && (IsWhiteSpaceLike(ch)))
                    {
                        if (hasPendingCommentRange && IsLineBreak(ch))
                            pendingHasTrailingNewLine = true;
                        pos++;
                        continue;
                    }
                    goto breakScan;
            }
        }

    breakScan:
        if (hasPendingCommentRange)
            accumulator = cb((pendingPos, pendingEnd, pendingKind, pendingHasTrailingNewLine, state, accumulator));

        return accumulator;
    }

    private static U ReduceEachLeadingCommentRange<T, U>(string text, int pos, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial)
    {
        return IterateCommentRanges(reduce: true, text, pos, trailing: false, cb, state, initial);
    }

    private static U ReduceEachTrailingCommentRange<T, U>(string text, int pos, Func<(int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, T state, U memo), U> cb, T state, U initial)
    {
        return IterateCommentRanges(reduce: true, text, pos, trailing: true, cb, state, initial);
    }

    private static List<CommentRange> AppendCommentRange((int pos, int end, SyntaxKind kind, bool hasTrailingNewLine, object _state, List<CommentRange> comments) cb)
    {
        cb.comments ??= [];
        cb.comments.Add(new CommentRange { Kind = cb.kind, Pos = cb.pos, End = cb.end, HasTrailingNewLine = cb.hasTrailingNewLine });
        return cb.comments;
    }

    internal static List<CommentRange> GetLeadingCommentRanges(string text, int pos)
    {
        return ReduceEachLeadingCommentRange<object, List<CommentRange>>(text, pos, AppendCommentRange, null, null) ?? [];
    }

    internal static List<CommentRange> GetTrailingCommentRanges(string text, int pos)
    {
        return ReduceEachTrailingCommentRange<object, List<CommentRange>>(text, pos, AppendCommentRange, null, null) ?? [];
    }


#if ISSOURCEGENERATOR
    private static readonly Regex commentDirectiveRegExSingleLine = new(@"^\/\/\/?\s*@(ts-expect-error|ts-ignore)/", RegexOptions.Compiled);
    private static readonly Regex commentDirectiveRegExMultiLine = new(@"^\/\/\/?\s*@(ts-expect-error|ts-ignore)/", RegexOptions.Compiled);
#else
    // Test for whether a single line comment with leading whitespace trimmed's text contains a directive.
    private static readonly Regex commentDirectiveRegExSingleLine = commentDirectiveReExSingleLineGen();

    // Test for whether a multi-line comment with leading whitespace trimmed's last line contains a directive.
    private static readonly Regex commentDirectiveRegExMultiLine = commentDirectiveRegExMultiLineGen();

    [GeneratedRegex(@"^\/\/\/?\s*@(ts-expect-error|ts-ignore)/")]
    private static partial Regex commentDirectiveReExSingleLineGen();

    [GeneratedRegex(@"^(?:\\/|\\*)*\\s*@(ts-expect-error|ts-ignore)")]
    private static partial Regex commentDirectiveRegExMultiLineGen();
#endif
}