namespace Serenity.TypeScript;

public class ArrayBindingPattern(NodeArray<IArrayBindingElement> elements)
    : Node(SyntaxKind.ArrayBindingPattern), IBindingPattern, IGetRestChildren
{
    public NodeArray<IArrayBindingElement> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}
