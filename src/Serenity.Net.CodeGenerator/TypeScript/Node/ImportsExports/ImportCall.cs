namespace Serenity.TypeScript;

internal class ImportCall(ImportExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}
