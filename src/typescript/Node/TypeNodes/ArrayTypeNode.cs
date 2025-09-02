namespace Serenity.TypeScript;

public class ArrayTypeNode(ITypeNode elementType)
    : TypeNodeBase(SyntaxKind.ArrayType), IGetRestChildren
{
    public ITypeNode ElementType { get; set; } = elementType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [ElementType];
    }
}