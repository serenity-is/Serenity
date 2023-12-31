namespace Serenity.TypeScript;

internal class UpdateExpressionBase(SyntaxKind kind)
    : UnaryExpressionBase(kind), IUpdateExpression
{
}