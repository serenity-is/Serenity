namespace Serenity.TypeScript;

internal class RegularExpressionLiteral(string text)
    : LiteralExpressionBase(SyntaxKind.RegularExpressionLiteral, text)
{
}
