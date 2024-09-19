
namespace Serenity.TypeScript;

public class WithStatement(IExpression expression, IStatement statement)
    : Statement(SyntaxKind.WithStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public IStatement Statement { get; } = statement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Statement];
    }
}
