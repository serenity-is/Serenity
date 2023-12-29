namespace Serenity.TypeScript;

internal class JsxElement(IJsxHasTagName openingElement, NodeArray<IJsxChild> children, IJsxHasTagName closingElement) 
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

internal class JsxAttributes(NodeArray<IObjectLiteralElement> properties) 
    : ObjectLiteralExpressionBase<IObjectLiteralElement>(SyntaxKind.JsxAttributes, properties) // JsxAttributeLike>
{
}

internal class JsxSelfClosingElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments, JsxAttributes attributes) 
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

internal class JsxOpeningElement(IJsxTagNameExpression tagName, NodeArray<ITypeNode> typeArguments,
    JsxAttributes attributes) : ExpressionBase(SyntaxKind.JsxOpeningElement), 
    IJsxOpeningLikeElement, IJsxHasTagName, IGetRestChildren
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

internal class JsxAttribute(IJsxAttributeName name, IExpression initializer) 
    : ObjectLiteralElement<IJsxAttributeName>(SyntaxKind.JsxAttribute, name), IGetRestChildren
{
    public IExpression Initializer { get; } = initializer; // StringLiteral | JsxExpression

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Initializer];
    }
}

internal class JsxSpreadAttribute(IExpression expression) 
    : ObjectLiteralElement<IPropertyName>(SyntaxKind.JsxSpreadAttribute, name: null), IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Name, Expression];
    }
}

internal class JsxClosingElement(IJsxTagNameExpression tagName)
    : Node(SyntaxKind.JsxClosingElement), IJsxHasTagName, IGetRestChildren
{
    public IJsxTagNameExpression TagName { get; } = tagName;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TagName];
    }
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

internal class JsxNamespacedName(Identifier name, Identifier @namespace)
    : Node(SyntaxKind.JsxNamespacedName), IJsxTagNameExpression, IJsxAttributeName, IHasNameProperty, IGetRestChildren
{
    public Identifier Name { get; } = name;
    public Identifier Namespace { get; } = @namespace;

    IDeclarationName IHasNameProperty.Name => Name;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Name, Namespace];
    }

}