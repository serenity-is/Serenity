namespace Serenity.TypeScript;

public class YieldExpression(AsteriskToken asteriskToken, IExpression expression)
    : ExpressionBase(SyntaxKind.YieldExpression), IGetRestChildren
{
    public AsteriskToken AsteriskToken { get; } = asteriskToken;
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [AsteriskToken, Expression];
    }
}
