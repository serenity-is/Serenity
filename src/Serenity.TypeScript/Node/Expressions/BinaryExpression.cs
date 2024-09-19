namespace Serenity.TypeScript;

public class BinaryExpression(IExpression left, Token operatorToken, IExpression right)
    : ExpressionBase(SyntaxKind.BinaryExpression), IExpression, IDeclaration, IGetRestChildren
{
    public IExpression Left { get; } = left;
    public /*BinaryOperator*/Token OperatorToken { get; } = operatorToken;
    public IExpression Right { get; } = right;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Left, OperatorToken, Right];
    }
}
