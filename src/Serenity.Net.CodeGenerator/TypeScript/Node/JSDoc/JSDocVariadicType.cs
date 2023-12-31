namespace Serenity.TypeScript;

internal class JSDocVariadicType(ITypeNode type)
    : JSDocTypeBase(SyntaxKind.JSDocVariadicType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}