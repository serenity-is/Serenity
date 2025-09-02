
namespace Serenity.TypeScript;

public class IfStatement(IExpression expression, IStatement thenStatement, IStatement elseStatement = null)
    : Statement(SyntaxKind.IfStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public IStatement ThenStatement { get; } = thenStatement;
    public IStatement ElseStatement { get; } = elseStatement;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, ThenStatement, ElseStatement];
    }
}
