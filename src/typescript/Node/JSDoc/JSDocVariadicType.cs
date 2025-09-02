namespace Serenity.TypeScript;

public class JSDocVariadicType(ITypeNode type)
    : JSDocTypeBase(SyntaxKind.JSDocVariadicType), IGetRestChildren
{
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}