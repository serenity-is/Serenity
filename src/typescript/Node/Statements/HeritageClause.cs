
namespace Serenity.TypeScript;

public class HeritageClause(SyntaxKind token, NodeArray<ExpressionWithTypeArguments> types)
    : Node(SyntaxKind.HeritageClause), IGetRestChildren
{
    public SyntaxKind Token { get; set; } = token; //  SyntaxKind.ExtendsKeyword | SyntaxKind.ImplementsKeyword
    public NodeArray<ExpressionWithTypeArguments> Types { get; } = types;

    public IEnumerable<INode> GetRestChildren()
    {
        return Types;
    }
}