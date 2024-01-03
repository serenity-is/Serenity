namespace Serenity.TypeScript;

public class Identifier : PrimaryExpressionBase, IDeclaration, IJsxTagNameExpression,
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