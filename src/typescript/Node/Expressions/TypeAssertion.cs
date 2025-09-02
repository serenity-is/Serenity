namespace Serenity.TypeScript;

public class TypeAssertion(ITypeNode type, IExpression expression)
    : UnaryExpressionBase(SyntaxKind.TypeAssertionExpression), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public /*UnaryExpression*/IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type, Expression];
    }
}
