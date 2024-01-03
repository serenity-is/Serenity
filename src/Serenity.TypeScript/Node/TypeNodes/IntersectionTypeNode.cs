namespace Serenity.TypeScript;

public class IntersectionTypeNode(NodeArray<ITypeNode> types)
    : TypeNodeBase(SyntaxKind.IntersectionType), IUnionOrIntersectionTypeNode, IGetRestChildren
{
    public NodeArray<ITypeNode> Types { get; set; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}
