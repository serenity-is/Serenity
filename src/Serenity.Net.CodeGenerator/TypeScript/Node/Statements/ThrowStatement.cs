
namespace Serenity.TypeScript;

internal class ThrowStatement(IExpression expression)
    : Statement(SyntaxKind.ThrowStatement), IFlowContainer, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
