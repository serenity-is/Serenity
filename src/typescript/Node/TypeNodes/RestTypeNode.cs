namespace Serenity.TypeScript;

public class RestTypeNode(ITypeNode type)
    : TypeNodeBase(SyntaxKind.RestType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
