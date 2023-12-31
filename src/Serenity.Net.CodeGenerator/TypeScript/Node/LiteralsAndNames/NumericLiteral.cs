namespace Serenity.TypeScript;

internal class NumericLiteral(string value, TokenFlags numericLiteralFlags = TokenFlags.None)
    : LiteralExpressionBase(SyntaxKind.NumericLiteral, value), IPropertyName
{
    public TokenFlags NumericLiteralFlags { get; set; } = numericLiteralFlags;
}
