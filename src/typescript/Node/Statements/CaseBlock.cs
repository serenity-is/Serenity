
namespace Serenity.TypeScript;

public class CaseBlock(NodeArray<ICaseOrDefaultClause> clauses)
    : Node(SyntaxKind.CaseBlock), IGetRestChildren
{
    public NodeArray<ICaseOrDefaultClause> Clauses { get; } = clauses;

    public IEnumerable<INode> GetRestChildren()
    {
        if (Clauses != null) foreach (var x in Clauses) yield return x;
    }
}
