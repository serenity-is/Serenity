namespace Serenity.TypeScript;

public class NamedImports(NodeArray<ImportSpecifier> elements)
    : Node(SyntaxKind.NamedImports), INamedImportsOrExports, INamedImportBindings, IGetRestChildren
{
    public NodeArray<ImportSpecifier> Elements { get; } = elements;

    public IEnumerable<INode> GetRestChildren()
    {
        return Elements;
    }
}
