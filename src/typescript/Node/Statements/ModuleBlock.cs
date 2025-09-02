
namespace Serenity.TypeScript;

public class ModuleBlock(NodeArray<IStatement> statements)
    : Node(SyntaxKind.ModuleBlock), IBlockLike, IGetRestChildren, INamespaceBody
{
    public NodeArray<IStatement> Statements { get; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}