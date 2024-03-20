namespace Serenity.TypeScript;

partial class Scanner
{
    private static bool IsIdentifierStart(int ch, ScriptTarget _)
    {
        return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || ch == '$' || ch == '_' ||
            (ch > CharacterCodes.MaxAsciiCharacter && IsUnicodeIdentifierStart(ch));
    }

    private static bool IsIdentifierPart(int ch, ScriptTarget _, LanguageVariant identifierVariant = LanguageVariant.Standard)
    {
        return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') ||
            (ch >= '0' && ch <= '9') || ch == '$' || ch == '_' ||
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
        return 1;
    }

    internal static bool IsIdentifierText(string name, ScriptTarget languageVersion, LanguageVariant identifierVariant = LanguageVariant.Standard)
    {
        if (string.IsNullOrEmpty(name)) 
            return false;
        var ch = (int)name[0];
        if (!IsIdentifierStart(ch, languageVersion))
        {
            return false;
        }

        for (var i = CharSize(ch); i < name.Length; i += CharSize(ch))
        {
            if (!IsIdentifierPart(name[i], languageVersion, identifierVariant))
            {
                return false;
            }
        }

        return true;
    }
}