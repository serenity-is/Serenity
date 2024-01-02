namespace Serenity.TypeScript;

public class NonNullExpression(IExpression expression) :
    LeftHandSideExpressionBase(SyntaxKind.NonNullExpression), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
