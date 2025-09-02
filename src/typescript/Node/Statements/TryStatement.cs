
namespace Serenity.TypeScript;

public class TryStatement(Block tryBlock, CatchClause catchClause, Block finallyBlock)
    : Statement(SyntaxKind.TryStatement), IFlowContainer, IGetRestChildren
{
    public Block TryBlock { get; } = tryBlock;
    public CatchClause CatchClause { get; } = catchClause;
    public Block FinallyBlock { get; } = finallyBlock;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TryBlock, CatchClause, FinallyBlock];
    }
}
