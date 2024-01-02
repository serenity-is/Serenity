namespace Serenity.TypeScript;

public class NoSubstitutionTemplateLiteral(string text, string rawText, TokenFlags templateFlags)
    : LiteralExpressionBase(SyntaxKind.NoSubstitutionTemplateLiteral, text),
    IPropertyName, ITemplateLiteralLikeNode, IDeclaration, IStringLiteralLike
{
    public string RawText { get; set; } = rawText;
    public TokenFlags? TemplateFlags { get; set; } = templateFlags & TokenFlags.TemplateLiteralLikeFlags;
}
