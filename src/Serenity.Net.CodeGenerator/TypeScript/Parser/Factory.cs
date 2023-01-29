using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.TsParser;

public static class Factory
{
    public static INode SkipPartiallyEmittedExpressions(INode node)
    {
        while (node.Kind == SyntaxKind.PartiallyEmittedExpression)
            node = ((PartiallyEmittedExpression) node).Expression;


        return node;
    }
}