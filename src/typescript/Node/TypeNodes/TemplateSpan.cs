namespace Serenity.TypeScript;

public class TemplateSpan(IExpression expression, ITemplateLiteralLikeNode literal)
    : Node(SyntaxKind.TemplateSpan), IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public ITemplateLiteralLikeNode Literal { get; } = literal; // TemplateMiddle | TemplateTail

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, Literal];
    }
}
