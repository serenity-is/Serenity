namespace Serenity.TypeScript;

public class ConditionalExpression(IExpression condition, QuestionToken questionToken,
    IExpression whenTrue, ColonToken colonToken, IExpression whenFalse)
    : ExpressionBase(SyntaxKind.ConditionalExpression), IGetRestChildren
{
    public IExpression Condition { get; } = condition;
    public QuestionToken QuestionToken { get; } = questionToken;
    public IExpression WhenTrue { get; } = whenTrue;
    public ColonToken ColonToken { get; } = colonToken;
    public IExpression WhenFalse { get; } = whenFalse;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Condition, QuestionToken, WhenTrue, ColonToken, WhenFalse];
    }
}