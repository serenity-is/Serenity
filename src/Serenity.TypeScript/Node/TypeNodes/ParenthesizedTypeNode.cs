namespace Serenity.TypeScript;

public class ParenthesizedTypeNode(ITypeNode type)
    : TypeNodeBase(SyntaxKind.ParenthesizedType), IGetRestChildren
{
    public ITypeNode Type { get; set; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
