
namespace Serenity.TypeScript;

public class ForOfStatement(AwaitKeyword awaitKeyword, IForInitializer initializer,
    IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.ForOfStatement, statement), IFlowContainer, IGetRestChildren
{
    public AwaitKeyword AwaitKeyword { get; } = awaitKeyword;
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [AwaitKeyword, Initializer, Expression, Statement];
    }
}
