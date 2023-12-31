namespace Serenity.TypeScript;

internal class BigIntLiteral(string text) :
    LiteralExpressionBase(SyntaxKind.BigIntLiteral, text), IPropertyName
{
}