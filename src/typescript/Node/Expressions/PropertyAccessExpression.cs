namespace Serenity.TypeScript;

public class PropertyAccessExpression(IExpression expression, Identifier name)
    : NamedDeclaration<Identifier>(SyntaxKind.PropertyAccessExpression, name),
        IMemberExpression, IJsxTagNameExpression, IGetRestChildren
{
    public IExpression Expression { get; } = expression;
    public QuestionDotToken QuestionDotToken { get; set; }

    public override IEnumerable<INode> GetRestChildren()
    {
        return [Expression, QuestionDotToken, Name];
    }
}