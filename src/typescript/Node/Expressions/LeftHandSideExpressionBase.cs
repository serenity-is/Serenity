namespace Serenity.TypeScript;

public class LeftHandSideExpressionBase(SyntaxKind kind) 
    : UpdateExpressionBase(kind), ILeftHandSideExpression
{
}
