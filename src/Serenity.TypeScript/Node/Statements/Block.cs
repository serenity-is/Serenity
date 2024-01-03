
namespace Serenity.TypeScript;

public class Block(NodeArray<IStatement> statements, bool? multiLine)
    : Statement(SyntaxKind.Block), IBlockOrExpression, IBlockLike, IGetRestChildren
{
    public NodeArray<IStatement> Statements { get; } = statements;
    public bool? MultiLine { get; } = multiLine;

    public IEnumerable<INode> GetRestChildren()
    {
        return Statements;
    }
}
