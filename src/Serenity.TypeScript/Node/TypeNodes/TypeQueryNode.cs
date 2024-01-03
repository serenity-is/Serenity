namespace Serenity.TypeScript;

public class TypeQueryNode(IEntityName exprName, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.TypeQuery), IGetRestChildren
{
    public IEntityName ExprName { get; } = exprName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return ExprName;
        if (TypeArguments != null)
            foreach (var x in TypeArguments) yield return x;
    }
}