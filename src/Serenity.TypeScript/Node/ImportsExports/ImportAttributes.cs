namespace Serenity.TypeScript;

public class ImportAttributes(NodeArray<ImportAttribute> elements, bool multiLine,
    SyntaxKind token = SyntaxKind.WithKeyword) : Node(SyntaxKind.ImportAttributes), IGetRestChildren
{
    public NodeArray<ImportAttribute> Elements { get; } = elements;
    public bool MultiLine { get; } = multiLine;
    public SyntaxKind Token { get; } = token;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}