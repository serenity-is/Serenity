namespace Serenity.TypeScript;

internal static class Factory
{
    internal static ITemplateLiteralLikeNode CreateTemplateLiteralLikeNode(SyntaxKind kind, string text, string rawText, TokenFlags? templateFlags)
    {
        if (kind == SyntaxKind.NoSubstitutionTemplateLiteral)
            return new NoSubstitutionTemplateLiteral(text, rawText, templateFlags ?? TokenFlags.None);

        return new TemplateLiteralLikeNode(kind, text, rawText, templateFlags);
    }

    internal static ILiteralLikeNode CreateLiteralLikeNode(SyntaxKind kind, string text) {
        return kind switch
        {
            SyntaxKind.NumericLiteral => new NumericLiteral(text, numericLiteralFlags: 0),
            SyntaxKind.BigIntLiteral => new BigIntLiteral(text),
            SyntaxKind.StringLiteral => new StringLiteral(text, isSingleQuote: null),
            SyntaxKind.JsxText => new JsxText(text, containsOnlyTriviaWhiteSpaces: false),
            SyntaxKind.JsxTextAllWhiteSpaces => new JsxText(text, containsOnlyTriviaWhiteSpaces: true),
            SyntaxKind.RegularExpressionLiteral => new RegularExpressionLiteral(text),
            SyntaxKind.NoSubstitutionTemplateLiteral => CreateTemplateLiteralLikeNode(kind, text, rawText: null, templateFlags: 0),
            _ => null,
        };
    }
}