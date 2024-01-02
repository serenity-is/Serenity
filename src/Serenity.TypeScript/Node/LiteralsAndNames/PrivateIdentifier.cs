namespace Serenity.TypeScript;

public class PrivateIdentifier : Identifier
{
    public PrivateIdentifier(string text, string escapedText = null,
        SyntaxKind? originalKeywordKind = null, bool? hasExtendedUnicodeEscape = null)
        : base(text, originalKeywordKind, hasExtendedUnicodeEscape)
    {
        EscapedText = escapedText;
        Kind = SyntaxKind.PrivateIdentifier;
    }
}