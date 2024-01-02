namespace Serenity.TypeScript;

public class SatisfiesExpression(IExpression expression, ITypeNode type)
    : ExpressionBase(SyntaxKind.SatisfiesExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Type];
    }
}