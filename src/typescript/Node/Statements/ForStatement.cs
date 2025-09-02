
namespace Serenity.TypeScript;

public class ForStatement(IForInitializer initializer,
    IExpression condition, IExpression incrementor, IStatement statement)
    : IterationStatement(SyntaxKind.ForStatement, statement), IFlowContainer, IGetRestChildren
{
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Condition { get; } = condition;
    public IExpression Incrementor { get; } = incrementor;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Initializer, Condition, Incrementor, Statement];
    }
}

