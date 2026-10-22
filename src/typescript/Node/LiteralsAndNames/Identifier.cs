namespace Serenity.TypeScript;

public class Identifier : PrimaryExpressionBase, IDeclaration, IJsxTagNameExpression,
    IEntityName, IPropertyName, IBindingName, IJsxAttributeName, IHasLiteralText, IModuleName, IModuleExportName
{
    public Identifier(string? text, SyntaxKind? originalKeywordKind = null, bool? hasExtendedUnicodeEscape = null,
        SyntaxKind kind = SyntaxKind.Identifier, string? sourceText = null)
        : base(kind)
    {
        this.sourceText = sourceText;

        if (originalKeywordKind == null && !string.IsNullOrEmpty(text))
            originalKeywordKind = Scanner.StringToToken(text!);

        if (originalKeywordKind == SyntaxKind.Identifier)
            originalKeywordKind = null;

        Text = text;
        // Unused by this parser port; kept commented for comparison.
        // OriginalKeywordKind = originalKeywordKind;
        // HasExtendedUnicodeEscape = hasExtendedUnicodeEscape ?? false;

        // if (HasExtendedUnicodeEscape)
        //    Flags |= NodeFlags.IdentifierHasExtendedUnicodeEscape;
    }

    private string? text;
    private string? sourceText;

    public string? Text
    {
        // Identifiers parsed without escapes usually don't need their text materialized.
        // Pos/End and the source text can reconstruct it on demand, and the result is cached.
        get => text ??= this.GetText(sourceText);
        set => text = value;
    }
    // Unused by this parser port; kept commented for comparison.
    // public SyntaxKind? OriginalKeywordKind { get; set; }
    // public bool IsInJsDocNamespace { get; set; }
    // public bool HasExtendedUnicodeEscape { get; set; }

    string? escapedText;

    public string? EscapedText
    {
        get { return Text ?? escapedText; }
        set { escapedText = value; }
    }
}