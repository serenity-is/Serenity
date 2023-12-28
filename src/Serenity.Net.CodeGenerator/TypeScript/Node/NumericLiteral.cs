namespace Serenity.TypeScript;

internal class NumericLiteral : LiteralExpression, IPropertyName
{
    public NumericLiteral(string value, TokenFlags numericLiteralFlags = TokenFlags.None)
    {
        Kind = SyntaxKind.NumericLiteral;
        Text = value;
        NumericLiteralFlags = numericLiteralFlags;
    }

    public TokenFlags NumericLiteralFlags { get; set; }
}