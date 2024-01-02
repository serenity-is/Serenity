namespace Serenity.TypeScript;

public class TemplateLiteralTypeSpan(ITypeNode type, ITemplateLiteralLikeNode literal)
    : TypeNodeBase(SyntaxKind.TemplateLiteralTypeSpan), IGetRestChildren
{
    public ITypeNode Type { get; } = type;
    public ITemplateLiteralLikeNode Literal { get; } = literal;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type, Literal];
    }
}