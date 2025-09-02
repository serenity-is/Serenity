namespace Serenity.TypeScript;

public class UpdateExpressionBase(SyntaxKind kind)
    : UnaryExpressionBase(kind), IUpdateExpression
{
}