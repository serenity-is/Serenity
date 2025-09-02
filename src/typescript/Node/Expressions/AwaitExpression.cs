namespace Serenity.TypeScript;

public class AwaitExpression(IExpression expression)
    : UnaryExpressionBase(SyntaxKind.AwaitExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}