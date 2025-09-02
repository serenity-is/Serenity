namespace Serenity.TypeScript;

public class ImportCall(ImportExpression expression, NodeArray<ITypeNode> typeArguments,
    NodeArray<IExpression> argumentsArray) : CallExpression(expression, typeArguments, argumentsArray)
{
}
