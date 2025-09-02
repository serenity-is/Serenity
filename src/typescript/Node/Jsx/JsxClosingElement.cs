namespace Serenity.TypeScript;

public class JsxClosingElement(IJsxTagNameExpression tagName)
    : Node(SyntaxKind.JsxClosingElement), IJsxHasTagName, IGetRestChildren
{
    public IJsxTagNameExpression TagName { get; } = tagName;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TagName];
    }
}
