namespace Serenity.TypeScript;

public class ParenthesizedExpression(IExpression expression)
    : PrimaryExpressionBase(SyntaxKind.ParenthesizedExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
