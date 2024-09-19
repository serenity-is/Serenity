namespace Serenity.TypeScript;

public class TemplateLiteralTypeNode(ITemplateLiteralLikeNode head, NodeArray<TemplateLiteralTypeSpan> templateSpans)
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
