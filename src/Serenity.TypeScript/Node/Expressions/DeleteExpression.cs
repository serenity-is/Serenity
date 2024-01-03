namespace Serenity.TypeScript;

public class DeleteExpression(IExpression expression)
    : UnaryExpressionBase(SyntaxKind.DeleteExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}