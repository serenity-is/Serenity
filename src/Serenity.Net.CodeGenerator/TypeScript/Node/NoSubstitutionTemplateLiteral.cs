namespace Serenity.TypeScript;

internal class NoSubstitutionTemplateLiteral : LiteralExpression, IPropertyName
{
    internal NoSubstitutionTemplateLiteral(string text, string rawText, TokenFlags? templateFlags)
    {
        Kind = SyntaxKind.NoSubstitutionTemplateLiteral;
        Text = text;
        RawText = rawText;
        TemplateFlags = templateFlags & TokenFlags.TemplateLiteralLikeFlags;
    }

    public string RawText { get; set; }
    internal TokenFlags? TemplateFlags { get; set; }
}

