namespace Serenity.TypeScript;

internal class JSDocTypeExpression : Node, IGetRestChildren
{
    public JSDocTypeExpression()
    {
        Kind = SyntaxKind.JSDocTypeExpression;
    }

    public ITypeNode Type { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
