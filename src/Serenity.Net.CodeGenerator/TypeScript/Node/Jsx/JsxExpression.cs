namespace Serenity.TypeScript;

internal class JsxExpression(DotDotDotToken dotDotDotToken, IExpression expression)
    : ExpressionBase(SyntaxKind.JsxExpression), IJsxChild, IJsxAttributeValue, IGetRestChildren
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken; // Token<SyntaxKind.DotDotDotToken>
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [DotDotDotToken, Expression];
    }

}