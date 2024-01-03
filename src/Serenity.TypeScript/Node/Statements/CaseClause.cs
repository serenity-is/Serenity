
namespace Serenity.TypeScript;

public class CaseClause(IExpression expression, NodeArray<IStatement> statements)
    : Node(SyntaxKind.CaseClause), ICaseOrDefaultClause, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public NodeArray<IStatement> Statements { get; } = statements;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Expression;
        if (Statements != null) foreach (var x in Statements) yield return x;
    }
}
