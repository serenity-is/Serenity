namespace Serenity.TypeScript;

internal class ParenthesizedExpression(IExpression expression)
    : PrimaryExpressionBase(SyntaxKind.ParenthesizedExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
