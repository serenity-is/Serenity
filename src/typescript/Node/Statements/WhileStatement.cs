
namespace Serenity.TypeScript;

public class WhileStatement(IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.WhileStatement, statement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Statement];
    }
}
