namespace Serenity.TypeScript;

public class AssignmentExpression(IExpression left, IExpression right)
    : BinaryExpression(left, new Token(SyntaxKind.EqualsToken), right)
{
}
