namespace Serenity.TypeScript;

public class JsxSelfClosingElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments, JsxAttributes attributes)
    : PrimaryExpressionBase(SyntaxKind.JsxSelfClosingElement), IJsxOpeningLikeElement, IJsxHasTagName,
    IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue, IGetRestChildren
{
    public IJsxTagNameExpression TagName { get; } = tagName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public JsxAttributes Attributes { get; } = attributes;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return TagName;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        yield return Attributes;
    }
}