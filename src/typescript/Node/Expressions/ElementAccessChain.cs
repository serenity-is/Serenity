namespace Serenity.TypeScript;

public class ElementAccessChain : ElementAccessExpression
{
    public ElementAccessChain(IExpression superExpression, QuestionDotToken questionDotToken, IExpression argumentExpression)
        : base(superExpression, questionDotToken, argumentExpression)
    {
        Flags |= NodeFlags.OptionalChain;
    }
}
