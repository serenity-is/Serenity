namespace Serenity.TypeScript;

public class ExpressionBase(SyntaxKind kind) 
    : Node(kind), IExpression
{
}