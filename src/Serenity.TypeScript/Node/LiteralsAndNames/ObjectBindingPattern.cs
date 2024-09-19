namespace Serenity.TypeScript;

public class ObjectBindingPattern(NodeArray<IArrayBindingElement> elements)
    : Node(SyntaxKind.ObjectBindingPattern), IBindingPattern, IGetRestChildren
{
    public NodeArray<IArrayBindingElement> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}
