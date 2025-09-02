namespace Serenity.TypeScript;

public class ArrayLiteralExpression(NodeArray<IExpression> elements, bool multiLine)
    : PrimaryExpressionBase(SyntaxKind.ArrayLiteralExpression), IGetRestChildren
{
    public NodeArray<IExpression> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}