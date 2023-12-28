namespace Serenity.TypeScript;

internal class TemplateLiteralLikeNode : LiteralLikeNode
{
    internal TemplateLiteralLikeNode(SyntaxKind kind, string text, string rawText, TokenFlags? templateFlags)
        : base(kind, text)
    {
        RawText = rawText;
        TemplateFlags = (templateFlags ?? TokenFlags.None) & TokenFlags.TemplateLiteralLikeFlags;
    }

    public string RawText { get; set; }
    internal TokenFlags? TemplateFlags { get; set; }
}

internal class TemplateSpan : NodeBase
{
    public TemplateSpan(IExpression expression, TemplateLiteralLikeNode literal)
    {
        Kind = SyntaxKind.TemplateSpan;
        Expression = expression;
        Literal = literal;
    }

    public IExpression Expression { get; }
    public TemplateLiteralLikeNode Literal { get; } // TemplateMiddle | TemplateTail
}

internal class TemplateLiteralTypeSpan : TypeNodeBase
{
    public TemplateLiteralTypeSpan(ITypeNode type, TemplateLiteralLikeNode literal)
        : base(SyntaxKind.TemplateLiteralTypeSpan)
    {
        Type = type;
        Literal = literal;
    }

    public ITypeNode Type { get; }
    public TemplateLiteralLikeNode Literal { get; }
}

internal class TemplateLiteralTypeNode(TemplateLiteralLikeNode head, NodeArray<TemplateLiteralTypeSpan> templateSpans)
    : TypeNodeBase(SyntaxKind.TemplateLiteralType)
{
    public TemplateLiteralLikeNode Head { get; } = head;
    public NodeArray<TemplateLiteralTypeSpan> TemplateSpans { get; } = templateSpans;
}

internal class TemplateExpression(TemplateLiteralLikeNode head, NodeArray<TemplateSpan> templateSpans) : PrimaryExpressionBase(SyntaxKind.TemplateExpression)
{
    public TemplateLiteralLikeNode Head { get; } = head;
    public NodeArray<TemplateSpan> TemplateSpans { get; } = templateSpans;
}
