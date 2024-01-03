
namespace Serenity.TypeScript;

public class LabeledStatement(Identifier label, IStatement statement)
    : Statement(SyntaxKind.LabeledStatement), IFlowContainer, IGetRestChildren
{
    public Identifier Label { get; } = label;
    public IStatement Statement { get; } = statement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Label, Statement];
    }
}
