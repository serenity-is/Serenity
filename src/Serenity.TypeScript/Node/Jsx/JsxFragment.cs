namespace Serenity.TypeScript;

public class JsxFragment(JsxOpeningFragment openingFragment, NodeArray<IJsxChild> children,
    JsxClosingFragment closingFragment) : PrimaryExpressionBase(SyntaxKind.JsxFragment),
    IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue, IGetRestChildren
{
    public JsxOpeningFragment OpeningFragment { get; } = openingFragment;
    public NodeArray<IJsxChild> Children { get; } = children;
    public JsxClosingFragment ClosingFragment { get; } = closingFragment;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return OpeningFragment;
        if (Children != null) foreach (var x in Children) yield return x;
        yield return ClosingFragment;
    }
}