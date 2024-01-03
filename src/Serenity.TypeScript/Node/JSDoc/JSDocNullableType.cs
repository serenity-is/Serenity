namespace Serenity.TypeScript;

public class JSDocNullableType(ITypeNode type, bool postfix)
    : JSDocTypeBase(SyntaxKind.JSDocNullableType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public bool Postfix { get; } = postfix;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}