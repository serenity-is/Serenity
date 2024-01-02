namespace Serenity.TypeScript;

public class QualifiedName(IEntityName left, Identifier right)
    : Node(SyntaxKind.QualifiedName), IEntityName, IGetRestChildren
{
    public IEntityName Left { get; } = left;
    public Identifier Right { get; } = right;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Left, Right];
    }
}
