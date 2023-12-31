namespace Serenity.TypeScript;

internal class CallChain(IExpression expression, QuestionDotToken questionDotToken, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray)
    : CallExpression(expression, questionDotToken, typeArguments, argumentsArray)
{
}