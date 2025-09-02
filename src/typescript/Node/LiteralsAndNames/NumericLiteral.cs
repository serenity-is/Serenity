namespace Serenity.TypeScript;

public class NumericLiteral(string value, TokenFlags numericLiteralFlags = TokenFlags.None)
    : LiteralExpressionBase(SyntaxKind.NumericLiteral, value), IPropertyName
{
    public TokenFlags NumericLiteralFlags { get; set; } = numericLiteralFlags;
}
