namespace Serenity.TypeScript;

internal class JSDocNonNullableType(ITypeNode type, bool postfix)
    : JSDocTypeBase(SyntaxKind.JSDocNonNullableType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
