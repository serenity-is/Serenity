
namespace Serenity.TypeScript;

internal class TemplateLiteralLikeNode(SyntaxKind kind, string text, string rawText, TokenFlags? templateFlags)
    : LiteralLikeNode(kind, text), ITemplateLiteralLikeNode
{
    public string RawText { get; set; } = rawText;
    public TokenFlags? TemplateFlags { get; set; } = (templateFlags ?? TokenFlags.None) & TokenFlags.TemplateLiteralLikeFlags;
}

internal class TemplateSpan(IExpression expression, ITemplateLiteralLikeNode literal)
    : Node(SyntaxKind.TemplateSpan), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITemplateLiteralLikeNode Literal { get; } = literal; // TemplateMiddle | TemplateTail

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Literal];
    }
}

internal class TemplateLiteralTypeSpan(ITypeNode type, ITemplateLiteralLikeNode literal) 
    : TypeNodeBase(SyntaxKind.TemplateLiteralTypeSpan), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public ITemplateLiteralLikeNode Literal { get; } = literal;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type, Literal];
    }
}

internal class TemplateLiteralTypeNode(ITemplateLiteralLikeNode head, NodeArray<TemplateLiteralTypeSpan> templateSpans)
    : TypeNodeBase(SyntaxKind.TemplateLiteralType), IGetRestChildren
{
    public ITemplateLiteralLikeNode Head { get; } = head;
    public NodeArray<TemplateLiteralTypeSpan> TemplateSpans { get; } = templateSpans;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Head;
        if (TemplateSpans != null) foreach (var x in TemplateSpans) yield return x;
    }
}

internal class TemplateExpression(ITemplateLiteralLikeNode head, NodeArray<TemplateSpan> templateSpans) 
    : PrimaryExpressionBase(SyntaxKind.TemplateExpression), IGetRestChildren
{
    public ITemplateLiteralLikeNode Head { get; } = head;
    public NodeArray<TemplateSpan> TemplateSpans { get; } = templateSpans;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Head;
        if (TemplateSpans != null) foreach (var x in TemplateSpans) yield return x;
    }
}
