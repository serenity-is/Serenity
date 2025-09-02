namespace Serenity.TypeScript;

public class RegularExpressionLiteral(string text)
    : LiteralExpressionBase(SyntaxKind.RegularExpressionLiteral, text)
{
}
