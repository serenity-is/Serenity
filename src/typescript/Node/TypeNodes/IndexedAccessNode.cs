namespace Serenity.TypeScript;

public class IndexedAccessTypeNode(ITypeNode objectType, ITypeNode indexType)
    : TypeNodeBase(SyntaxKind.IndexedAccessType), IGetRestChildren
{
    public ITypeNode ObjectType { get; set; } = objectType;
    public ITypeNode IndexType { get; set; } = indexType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ObjectType, IndexType];
    }
}
