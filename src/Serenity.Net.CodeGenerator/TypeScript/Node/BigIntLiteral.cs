namespace Serenity.TypeScript;

internal class BigIntLiteral : LiteralExpression, IPropertyName
{
    public BigIntLiteral(string value)
    {
        Kind = SyntaxKind.BigIntLiteral;
        Text = value;
    }
}