namespace Serenity.TypeScript;

public class SuperCall(SuperExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}