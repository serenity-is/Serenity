namespace Serenity.TypeScript;

public class AsExpression(IExpression expression, ITypeNode type)
    : ExpressionBase(SyntaxKind.AsExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Type];
    }
}
