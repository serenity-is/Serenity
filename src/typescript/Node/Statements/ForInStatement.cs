
namespace Serenity.TypeScript;

public class ForInStatement(IForInitializer initializer, IExpression expression, IStatement statement)
    : IterationStatement(SyntaxKind.ForInStatement, statement), IFlowContainer, IGetRestChildren
{
    public IForInitializer Initializer { get; } = initializer;
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Initializer, Expression, Statement];
    }
}

