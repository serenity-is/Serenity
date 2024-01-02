
namespace Serenity.TypeScript;

public class DoStatement(IStatement statement, IExpression expression)
    : IterationStatement(SyntaxKind.DoStatement, statement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Statement, Expression];
    }
}
