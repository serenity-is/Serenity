namespace Serenity.TypeScript;

public class BigIntLiteral(string text) :
    LiteralExpressionBase(SyntaxKind.BigIntLiteral, text), IPropertyName
{
}