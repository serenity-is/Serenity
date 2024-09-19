namespace Serenity.TypeScript;

public class ExpressionWithTypeArguments(ILeftHandSideExpression expression, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.ExpressionWithTypeArguments), IMemberExpression, IGetRestChildren
{
    public ILeftHandSideExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
    }
}