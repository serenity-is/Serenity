namespace Serenity.TypeScript;

public class NewExpression(IExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : PrimaryExpressionBase(SyntaxKind.NewExpression), IDeclaration, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public NodeArray<IExpression> Arguments { get; } = argumentsArray;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        if (Arguments != null) foreach (var x in Arguments) yield return x;
    }
}
