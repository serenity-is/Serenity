namespace Serenity.TypeScript;

public class OptionalTypeNode(ITypeNode type)
    : TypeNodeBase(SyntaxKind.OptionalType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
