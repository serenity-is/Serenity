namespace Serenity.TypeScript;

partial class Scanner
{
    internal static class CharacterCodes
    {
        internal const int NullCharacter = 0;
        internal const int MaxAsciiCharacter = 0x7F;

        internal const int LineFeed = 0x0A;              // \n
        internal const int CarriageReturn = 0x0D;        // \r
        internal const int LineSeparator = 0x2028;
        internal const int ParagraphSeparator = 0x2029;
        internal const int NextLine = 0x0085;

        internal const int Space = 0x0020;
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

        internal const int _ = 0x5F;
        internal const int Dollar = 0x24;

        internal const int _0 = 0x30;
        internal const int _1 = 0x31;
        internal const int _2 = 0x32;
        internal const int _3 = 0x33;
        internal const int _4 = 0x34;
        internal const int _5 = 0x35;
        internal const int _6 = 0x36;
        internal const int _7 = 0x37;
        internal const int _8 = 0x38;
        internal const int _9 = 0x39;

        internal const int a = 0x61;
        internal const int b = 0x62;
        internal const int c = 0x63;
        internal const int d = 0x64;
        internal const int e = 0x65;
        internal const int f = 0x66;
        internal const int g = 0x67;
        internal const int h = 0x68;
        internal const int i = 0x69;
        internal const int j = 0x6A;
        internal const int k = 0x6B;
        internal const int l = 0x6C;
        internal const int m = 0x6D;
        internal const int n = 0x6E;
        internal const int o = 0x6F;
        internal const int p = 0x70;
        internal const int q = 0x71;
        internal const int r = 0x72;
        internal const int s = 0x73;
        internal const int t = 0x74;
        internal const int u = 0x75;
        internal const int v = 0x76;
        internal const int w = 0x77;
        internal const int x = 0x78;
        internal const int y = 0x79;
        internal const int z = 0x7A;

        internal const int A = 0x41;
        internal const int B = 0x42;
        internal const int C = 0x43;
        internal const int D = 0x44;
        internal const int E = 0x45;
        internal const int F = 0x46;
        internal const int G = 0x47;
        internal const int H = 0x48;
        internal const int I = 0x49;
        internal const int J = 0x4A;
        internal const int K = 0x4B;
        internal const int L = 0x4C;
        internal const int M = 0x4D;
        internal const int N = 0x4E;
        internal const int O = 0x4F;
        internal const int P = 0x50;
        internal const int Q = 0x51;
        internal const int R = 0x52;
        internal const int S = 0x53;
        internal const int T = 0x54;
        internal const int U = 0x55;
        internal const int V = 0x56;
        internal const int W = 0x57;
        internal const int X = 0x58;
        internal const int Y = 0x59;
        internal const int Z = 0x5a;

        internal const int Ampersand = 0x26;             // &
        internal const int Asterisk = 0x2A;              // *
        internal const int At = 0x40;                    // @
        internal const int Backslash = 0x5C;             // \
        internal const int Backtick = 0x60;              // `
        internal const int Bar = 0x7C;                   // |
        internal const int Caret = 0x5E;                 // ^
        internal const int CloseBrace = 0x7D;            // }
        internal const int CloseBracket = 0x5D;          // ]
        internal const int CloseParen = 0x29;            // )
        internal const int Colon = 0x3A;                 // :
        internal const int Comma = 0x2C;                 // ;
        internal const int Dot = 0x2E;                   // .
        internal const int DoubleQuote = 0x22;           // "
        internal new const int Equals = 0x3D;            // =
        internal const int Exclamation = 0x21;           // !
        internal const int GreaterThan = 0x3E;           // >
        internal const int Hash = 0x23;                  // #
        internal const int LessThan = 0x3C;              // <
        internal const int Minus = 0x2D;                 // -
        internal const int OpenBrace = 0x7B;             // {
        internal const int OpenBracket = 0x5B;           // [
        internal const int OpenParen = 0x28;             // (
        internal const int Percent = 0x25;               // %
        internal const int Plus = 0x2B;                  // +
        internal const int Question = 0x3F;              // ?
        internal const int Semicolon = 0x3B;             // ;
        internal const int SingleQuote = 0x27;           // '
        internal const int Slash = 0x2F;                 // /
        internal const int Tilde = 0x7E;                 // ~

        internal const int Backspace = 0x08;             // \b
        internal const int FormFeed = 0x0C;              // \f
        internal const int ByteOrderMark = 0xFEFF;
        internal const int Tab = 0x09;                   // \t
        internal const int VerticalTab = 0x0B;           // \v
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