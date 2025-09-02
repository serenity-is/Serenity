namespace Serenity.TypeScript;

public class UnaryExpressionBase(SyntaxKind kind) 
    : ExpressionBase(kind), IUnaryExpression
{
}