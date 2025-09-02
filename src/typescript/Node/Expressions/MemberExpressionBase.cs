namespace Serenity.TypeScript;

public class MemberExpressionBase(SyntaxKind kind) 
    : LeftHandSideExpressionBase(kind), IMemberExpression
{
}
