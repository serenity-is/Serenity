
namespace Serenity.TypeScript;

public class BreakStatement(Identifier label = null)
    : Statement(SyntaxKind.BreakStatement), IBreakOrContinueStatement, IFlowContainer, IGetRestChildren
{
    public Identifier Label { get; } = label;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Label];
    }
}
