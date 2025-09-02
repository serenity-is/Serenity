namespace Serenity.TypeScript;

public class NodeWithTypeArguments(SyntaxKind kind, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(kind), IGetRestChildren
{
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return TypeArguments ?? [];
    }
}
