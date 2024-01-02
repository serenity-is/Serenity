namespace Serenity.TypeScript;

public class TaggedTemplateExpression(IExpression tag, NodeArray<ITypeNode> typeArguments, INode template)
    : MemberExpressionBase(SyntaxKind.TaggedTemplateExpression), IGetRestChildren
{
    public IExpression Tag { get; set; } = tag; //LeftHandSideExpression
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public INode Template { get; set; } = template; //TemplateLiteral
    public QuestionDotToken QuestionDotToken { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Tag;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
        yield return Template;
        yield return QuestionDotToken;
    }
}