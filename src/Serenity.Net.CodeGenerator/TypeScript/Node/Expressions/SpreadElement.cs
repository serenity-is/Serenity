namespace Serenity.TypeScript;

internal class SpreadElement(IExpression expression)
    : ExpressionBase(SyntaxKind.SpreadElement), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
