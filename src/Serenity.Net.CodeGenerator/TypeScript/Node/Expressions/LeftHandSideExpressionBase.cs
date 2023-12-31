namespace Serenity.TypeScript;

internal class LeftHandSideExpressionBase(SyntaxKind kind) 
    : UpdateExpressionBase(kind), ILeftHandSideExpression
{
}
