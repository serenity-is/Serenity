namespace Serenity.TypeScript;

public class TemplateExpression(ITemplateLiteralLikeNode head, NodeArray<TemplateSpan> templateSpans)
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
