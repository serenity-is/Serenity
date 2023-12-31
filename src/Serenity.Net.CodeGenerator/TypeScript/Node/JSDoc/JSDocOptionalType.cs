namespace Serenity.TypeScript;

internal class JSDocOptionalType(ITypeNode type)
    : JSDocTypeBase(SyntaxKind.JSDocOptionalType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}