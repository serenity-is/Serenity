namespace Serenity.TypeScript;

internal class AssignmentExpression(IExpression left, IExpression right)
    : BinaryExpression(left, new Token(SyntaxKind.EqualsToken), right)
{
}
