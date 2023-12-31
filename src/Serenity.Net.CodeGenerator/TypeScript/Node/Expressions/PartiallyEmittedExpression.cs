namespace Serenity.TypeScript;

internal class PartiallyEmittedExpression(IExpression expression)
    : LeftHandSideExpressionBase(SyntaxKind.PartiallyEmittedExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}