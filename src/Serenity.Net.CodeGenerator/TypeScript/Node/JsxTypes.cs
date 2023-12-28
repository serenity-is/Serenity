namespace Serenity.TypeScript;

internal class JsxElement(IJsxHasTagName openingElement, NodeArray<IJsxChild> children, IJsxHasTagName closingElement) 
    : PrimaryExpressionBase(SyntaxKind.JsxElement), IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue
{
    public IJsxHasTagName OpeningElement { get; } = openingElement;
    public NodeArray<IJsxChild> Children { get; } = children;
    public IJsxHasTagName ClosingElement { get; } = closingElement;
}

internal class JsxAttributes(NodeArray<IObjectLiteralElement> properties) : ObjectLiteralExpressionBase<IObjectLiteralElement>(SyntaxKind.JsxAttributes, properties) // JsxAttributeLike>
{
}

internal class JsxSelfClosingElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments, JsxAttributes attributes) 
    : PrimaryExpressionBase(SyntaxKind.JsxSelfClosingElement), IJsxOpeningLikeElement, IJsxHasTagName, 
    IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue
{
    public IJsxTagNameExpression TagName { get; } = tagName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public JsxAttributes Attributes { get; } = attributes;
}


internal class JsxOpeningElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments,
    JsxAttributes attributes) : ExpressionBase(SyntaxKind.JsxOpeningElement), 
    IJsxOpeningLikeElement, IJsxHasTagName
{
    public IJsxTagNameExpression TagName { get; } = tagName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public JsxAttributes Attributes { get; } = attributes;
}

internal class JsxExpression(DotDotDotToken dotDotDotToken, IExpression expression) 
    : ExpressionBase(SyntaxKind.JsxExpression), IJsxChild, IJsxAttributeValue
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken; // Token<SyntaxKind.DotDotDotToken>
    public IExpression Expression { get; } = expression;
}

internal class JsxAttribute(IJsxAttributeName name, IExpression initializer) 
    : ObjectLiteralElement(SyntaxKind.JsxAttribute, name)
{
    public IExpression Initializer { get; } = initializer; // StringLiteral | JsxExpression
}

internal class JsxSpreadAttribute(IExpression expression) : ObjectLiteralElement(SyntaxKind.JsxSpreadAttribute, name: null)
{
    public IExpression Expression { get; } = expression;
}

internal class JsxClosingElement(IJsxTagNameExpression tagName) : NodeBase(SyntaxKind.JsxClosingElement), IJsxHasTagName
{
    public IJsxTagNameExpression TagName { get; } = tagName;
}

internal class JsxText(string text, bool containsOnlyTriviaWhiteSpaces)
    : LiteralLikeNode(SyntaxKind.JsxText, text), IJsxChild
{
    public bool ContainsOnlyTriviaWhiteSpaces { get; } = containsOnlyTriviaWhiteSpaces;
}


internal class JsxOpeningFragment()
    : ExpressionBase(SyntaxKind.JsxOpeningFragment), IJsxOpeningLikeElementOrOpeningFragment
{
}

internal class JsxClosingFragment()
    : ExpressionBase(SyntaxKind.JsxClosingFragment)
{
}

internal class JsxFragment(JsxOpeningFragment openingFragment, NodeArray<IJsxChild> children, 
    JsxClosingFragment closingFragment) : PrimaryExpressionBase(SyntaxKind.JsxFragment), 
    IJsxElementOrSelfClosingOrFragment, IJsxAttributeValue
{
    public JsxOpeningFragment OpeningFragment { get; } = openingFragment;
    public NodeArray<IJsxChild> Children { get; } = children;
    public JsxClosingFragment ClosingFragment { get; } = closingFragment;
}

internal class JsxNamespacedName(Identifier name, Identifier @namespace)
    : NodeBase(SyntaxKind.JsxNamespacedName), IJsxTagNameExpression, IJsxAttributeName
{
    public Identifier Name { get; } = name;
    public Identifier Namespace { get; } = @namespace;
}