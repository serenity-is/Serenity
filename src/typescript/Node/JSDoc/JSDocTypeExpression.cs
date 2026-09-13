namespace Serenity.TypeScript;

public class JSDocTypeExpression() : Node(SyntaxKind.JSDocTypeExpression), IGetRestChildren
{
    public ITypeNode? Type { get; set; }

    public IEnumerable<INode?> GetRestChildren()
    {
        return [Type];
    }
}
