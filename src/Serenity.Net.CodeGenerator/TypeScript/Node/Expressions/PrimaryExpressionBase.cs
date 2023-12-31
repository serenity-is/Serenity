namespace Serenity.TypeScript;

internal class PrimaryExpressionBase(SyntaxKind kind)
    : MemberExpressionBase(kind), IPrimaryExpression, IJsxTagNameExpression
{
}