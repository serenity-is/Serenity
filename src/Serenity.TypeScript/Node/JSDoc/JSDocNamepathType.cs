namespace Serenity.TypeScript;

public class JSDocNamepathType(ITypeNode type)
    : JSDocTypeBase(SyntaxKind.JSDocNamepathType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Type;
    }
}