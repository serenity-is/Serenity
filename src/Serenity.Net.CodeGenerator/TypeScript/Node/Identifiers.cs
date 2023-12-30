namespace Serenity.TypeScript;

internal class Identifier : PrimaryExpressionBase, IDeclaration, IJsxTagNameExpression, 
    IEntityName, IPropertyName, IBindingName, IJsxAttributeName, IHasLiteralText, IModuleName
{
    public Identifier(string text, SyntaxKind? originalKeywordKind = null, bool? hasExtendedUnicodeEscape = null)
        : base(SyntaxKind.Identifier)
    {
        if (originalKeywordKind == null && !string.IsNullOrEmpty(text))
            originalKeywordKind = Scanner.StringToToken(text);

        if (originalKeywordKind == SyntaxKind.Identifier)
            originalKeywordKind = null;

        Text = text;
        OriginalKeywordKind = originalKeywordKind;
        HasExtendedUnicodeEscape = hasExtendedUnicodeEscape ?? false;

        if (HasExtendedUnicodeEscape) 
            Flags |= NodeFlags.IdentifierHasExtendedUnicodeEscape;
    }

    public string Text { get; set; }
    public SyntaxKind? OriginalKeywordKind { get; set; }
    public bool IsInJsDocNamespace { get; set; }
    public bool HasExtendedUnicodeEscape { get; set; }

    string escapedText;

    public string EscapedText
    {
        get { return Text ?? escapedText; }
        set { escapedText = value; }
    }
}

internal class PrivateIdentifier : Identifier
{
    public PrivateIdentifier(string text, string escapedText = null, 
        SyntaxKind? originalKeywordKind = null, bool? hasExtendedUnicodeEscape = null)
        : base(text, originalKeywordKind, hasExtendedUnicodeEscape)
    {
        EscapedText = escapedText;
        Kind = SyntaxKind.PrivateIdentifier;
    }
}

internal class QualifiedName(IEntityName left, Identifier right)
    : Node(SyntaxKind.QualifiedName), IEntityName, IGetRestChildren
{
    public IEntityName Left { get; } = left;
    public Identifier Right { get; } = right;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Left, Right];
    }
}


internal class ComputedPropertyName(IExpression expression)
    : Node(SyntaxKind.ComputedPropertyName), IPropertyName, IGetRestChildren
{
    public IExpression Expression { get; } = expression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression];
    }
}
