namespace Serenity.TypeScript;

public class JsxElement(IJsxHasTagName openingElement, NodeArray<IJsxChild> children, IJsxHasTagName closingElement)
    : PrimaryExpressionBase(SyntaxKind.JsxElement), IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue, IGetRestChildren
{
    public IJsxHasTagName OpeningElement { get; } = openingElement;
    public NodeArray<IJsxChild> Children { get; } = children;
    public IJsxHasTagName ClosingElement { get; } = closingElement;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return OpeningElement;
        if (Children != null) foreach (var x in Children) yield return x;
        yield return ClosingElement;
    }
}