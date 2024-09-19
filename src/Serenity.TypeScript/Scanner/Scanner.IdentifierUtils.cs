namespace Serenity.TypeScript;

partial class Scanner
{
    private static bool IsIdentifierStart(int ch, ScriptTarget _)
    {
        return IsASCIILetter(ch) || ch == '$' || ch == '_' ||
            (ch > CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierStart(ch));
    }

    private static bool IsIdentifierPart(int ch, ScriptTarget _, LanguageVariant identifierVariant = LanguageVariant.Standard)
    {
        return IsWordCharacter(ch) || ch == '$' ||
            // "-" and ":" are valid in JSX Identifiers
            (identifierVariant == LanguageVariant.JSX && (ch == CharacterCodes.Minus || ch == CharacterCodes.Colon)) ||
            (ch > CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierPart(ch));
    }

    private static int CharSize(int ch)
    {
        if (ch >= 0x10000)
        {
            return 2;
        }
        if (ch == CharacterCodes.EOF)
        {
            return 0;
        }
        return 1;
    }

    internal static int CodePointAt(string s, int pos)
    {
        if (pos < 0 || pos >= s.Length)
            return CharacterCodes.EOF;

        if (pos < s.Length - 1 &&
            char.IsSurrogatePair(s, pos))
            return char.ConvertToUtf32(s, pos);

        return s[pos];
    }

    internal static bool IsIdentifierText(string name, ScriptTarget languageVersion, LanguageVariant identifierVariant = LanguageVariant.Standard)
    {
        if (string.IsNullOrEmpty(name)) 
            return false;
        var ch = CodePointAt(name, 0);
        if (!IsIdentifierStart(ch, languageVersion))
        {
            return false;
        }

        for (var i = CharSize(ch); i < name.Length; i += CharSize(ch))
        {
            if (!IsIdentifierPart(CodePointAt(name, i), languageVersion, identifierVariant))
            {
                return false;
            }
        }

        return true;
    }
}