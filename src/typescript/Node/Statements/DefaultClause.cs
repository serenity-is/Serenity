
namespace Serenity.TypeScript;

public class DefaultClause(NodeArray<IStatement> statements)
    : Node(SyntaxKind.DefaultClause), ICaseOrDefaultClause, IGetRestChildren
{
    public NodeArray<IStatement> Statements { get; set; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}