namespace Serenity.TypeScript;

public class TemplateLiteralLikeNode(SyntaxKind kind, string text, string rawText, TokenFlags? templateFlags)
    : LiteralLikeNode(kind, text), ITemplateLiteralLikeNode
{
    public string RawText { get; set; } = rawText;
    public TokenFlags? TemplateFlags { get; set; } = (templateFlags ?? TokenFlags.None) & TokenFlags.TemplateLiteralLikeFlags;
}
