namespace Serenity.TypeScript;

public class VariableDeclarationList(NodeArray<VariableDeclaration> declarations)
    : Node(SyntaxKind.VariableDeclarationList), IVariableDeclarationList, IGetRestChildren, IForInitializer
{
    public VariableDeclarationList(NodeArray<VariableDeclaration> declarations, NodeFlags nodeFlags)
        : this(declarations)
    {
        Flags &= nodeFlags;
    }

    public NodeArray<VariableDeclaration> Declarations { get; } = declarations;
    public NodeFlags NodeFlags { get; }

    public IEnumerable<INode> GetRestChildren()
    {
        return Declarations;
    }
}
