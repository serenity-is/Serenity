namespace Serenity.TypeScript;

internal class JsxElement(IExpression openingElement, NodeArray<IJsxChild> children, JsxClosingElement closingElement) : PrimaryExpression(SyntaxKind.JsxElement), IJsxChild
{
    public /*JsxOpeningElement*/IExpression OpeningElement { get; } = openingElement;
    public NodeArray<IJsxChild> Children { get; } = children;
    public JsxClosingElement ClosingElement { get; } = closingElement;
}

internal class JsxAttributes(NodeArray<ObjectLiteralElement> properties) : ObjectLiteralExpressionBase<ObjectLiteralElement>(SyntaxKind.JsxAttributes, properties) // JsxAttributeLike>
{
}

internal class JsxSelfClosingElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments,
    JsxAttributes attributes) : PrimaryExpression(SyntaxKind.JsxSelfClosingElement), IJsxChild
{
    public IJsxTagNameExpression TagName { get; } = tagName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public JsxAttributes Attributes { get; } = attributes;
}

internal class JsxOpeningElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments,
    JsxAttributes attributes) : Expression(SyntaxKind.JsxOpeningElement)
{
    public IJsxTagNameExpression TagName { get; } = tagName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;
    public JsxAttributes Attributes { get; } = attributes;
}

internal class JsxExpression(DotDotDotToken dotDotDotToken, IExpression expression) : Expression(SyntaxKind.JsxExpression), IJsxChild
{
    public DotDotDotToken DotDotDotToken { get; } = dotDotDotToken; // Token<SyntaxKind.DotDotDotToken>
    public IExpression Expression { get; } = expression;
}

internal class JsxAttribute(IDeclarationName name, IExpression initializer) 
    : ObjectLiteralElement(SyntaxKind.JsxAttribute, name)
{
    public IExpression Initializer { get; } = initializer; // StringLiteral | JsxExpression
}

internal class JsxSpreadAttribute(IExpression expression) : ObjectLiteralElement(SyntaxKind.JsxSpreadAttribute, name: null)
{
    public IExpression Expression { get; } = expression;
}

internal class JsxClosingElement(IJsxTagNameExpression tagName) : NodeBase(SyntaxKind.JsxClosingElement)
{
    public IJsxTagNameExpression TagName { get; } = tagName;
}

internal class JsxText(string text, bool containsOnlyTriviaWhiteSpaces)
    : LiteralLikeNode(SyntaxKind.JsxText, text), IJsxChild
{
    public bool ContainsOnlyTriviaWhiteSpaces { get; } = containsOnlyTriviaWhiteSpaces;
}
