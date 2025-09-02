namespace Serenity.TypeScript;

public class UnionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.UnionType), IUnionOrIntersectionTypeNode, IGetRestChildren
{
    public NodeArray<ITypeNode> Types { get; set; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}
