namespace Serenity.TypeScript;

public class VoidExpression(IExpression expression) 
    : UnaryExpressionBase(SyntaxKind.VoidExpression), IGetRestChildren
{
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
