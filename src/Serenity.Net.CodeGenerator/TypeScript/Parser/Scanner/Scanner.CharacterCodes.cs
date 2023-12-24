namespace Serenity.TypeScript.TsParser;

partial class Scanner
{
    internal static class CharacterCodes
    {
        internal const int NullCharacter = 0;
        internal const int MaxAsciiCharacter = 0x7F;

        internal const int LineSeparator = 0x2028;
        internal const int ParagraphSeparator = 0x2029;
        internal const int NextLine = 0x0085;

        internal const int NonBreakingSpace = 0x00A0;
        internal const int EnQuad = 0x2000;
        internal const int EmQuad = 0x2001;
        internal const int EnSpace = 0x2002;
        internal const int EmSpace = 0x2003;
        internal const int ThreePerEmSpace = 0x2004;
        internal const int FourPerEmSpace = 0x2005;
        internal const int SixPerEmSpace = 0x2006;
        internal const int FigureSpace = 0x2007;
        internal const int PunctuationSpace = 0x2008;
        internal const int ThinSpace = 0x2009;
        internal const int HairSpace = 0x200A;
        internal const int ZeroWidthSpace = 0x200B;
        internal const int NarrowNoBreakSpace = 0x202F;
        internal const int IdeographicSpace = 0x3000;
        internal const int MathematicalSpace = 0x205F;
        internal const int Ogham = 0x1680;

        // Unicode replacement character produced when a byte sequence is invalid
        internal const int ReplacementCharacter = 0xFFFD;

        internal const int ByteOrderMark = 0xFEFF;
    }

    private static bool IsCodePoint(int code)
    {
        return code <= 0x10FFFF;
    }

    private static bool IsDigit(int ch)
    {
        return ch >= '0' && ch <= '9';
    }

    private static bool IsHexDigit(int ch)
    {
        return IsDigit(ch) || (ch >= 'A' && ch <= 'F') || ch >= 'a' && ch <= 'f';
    }

    internal static bool IsOctalDigit(int ch)
    {
        return ch >= '0' && ch <= '7';
    }

    private static bool IsLineBreak(int ch)
    {
        // ES5 7.3:
        // The ECMAScript line terminator characters are listed in Table 3.
        //     Table 3: Line Terminator Characters
        //     Code Unit Value     Name                    Formal Name
        //     \u000A              Line Feed               <LF>
        //     \u000D              Carriage Return         <CR>
        //     \u2028              Line separator          <LS>
        //     \u2029              Paragraph separator     <PS>
        // Only the characters in Table 3 are treated as line terminators. Other new line or line
        // breaking characters are treated as white space but not as line terminators.

        return ch == '\n' ||
            ch == '\r' ||
            ch == CharacterCodes.LineSeparator ||
            ch == CharacterCodes.ParagraphSeparator;
    }

    public static bool IsWhiteSpaceLike(int ch)
    {
        return IsWhiteSpaceSingleLine(ch) || IsLineBreak(ch);
    }

    public static bool IsWhiteSpaceSingleLine(int ch)
    {
        // Note: nextLine is in the Zs space, and should be considered to be a whitespace.
        // It is explicitly not a line-break as it isn't in the exact set specified by EcmaScript.
        return ch == ' ' ||
            ch == '\t' ||
            ch == '\v' ||
            ch == '\f' ||
            ch == CharacterCodes.NonBreakingSpace ||
            ch == CharacterCodes.NextLine ||
            ch == CharacterCodes.Ogham ||
            ch >= CharacterCodes.EnQuad && ch <= CharacterCodes.ZeroWidthSpace ||
            ch == CharacterCodes.NarrowNoBreakSpace ||
            ch == CharacterCodes.MathematicalSpace ||
            ch == CharacterCodes.IdeographicSpace ||
            ch == CharacterCodes.ByteOrderMark;
    }
}