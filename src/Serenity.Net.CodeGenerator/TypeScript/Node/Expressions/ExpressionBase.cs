namespace Serenity.TypeScript;

internal class ExpressionBase(SyntaxKind kind) 
    : Node(kind), IExpression
{
}