namespace Serenity.TypeScript;

public class PropertyAccessChain : PropertyAccessExpression
{
    public PropertyAccessChain(IExpression expression, QuestionDotToken questionDotToken, Identifier name)
        : base(expression, name)
    {
        Flags |= NodeFlags.OptionalChain;
        QuestionDotToken = questionDotToken;
    }
}
