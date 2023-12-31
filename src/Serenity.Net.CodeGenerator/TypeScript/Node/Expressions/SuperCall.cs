namespace Serenity.TypeScript;

internal class SuperCall(SuperExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}