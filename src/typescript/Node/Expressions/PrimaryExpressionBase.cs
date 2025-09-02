namespace Serenity.TypeScript;

public class PrimaryExpressionBase(SyntaxKind kind)
    : MemberExpressionBase(kind), IPrimaryExpression, IJsxTagNameExpression
{
}