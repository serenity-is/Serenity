namespace Serenity.TypeScript;

public class ElementAccessExpression(IExpression expression, IExpression argumentExpression)
    : MemberExpressionBase(SyntaxKind.ElementAccessExpression), IGetRestChildren
{
    public ElementAccessExpression(IExpression expression, QuestionDotToken questionDotToken, IExpression argumentExpression)
        : this(expression, argumentExpression)
    {
        QuestionDotToken = questionDotToken;
    }

    public IExpression Expression { get; } = expression; //LeftHandSideExpression
    public QuestionDotToken QuestionDotToken { get; }
    public IExpression ArgumentExpression { get; } = argumentExpression;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Expression, QuestionDotToken, ArgumentExpression];
    }
}