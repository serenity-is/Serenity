
namespace Serenity.TypeScript;

public class ReturnStatement(IExpression expression)
    : Statement(SyntaxKind.ReturnStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
