namespace Serenity.TypeScript;

public class TypeOperatorNode(SyntaxKind op, ITypeNode type)
    : TypeNodeBase(SyntaxKind.TypeOperator), IGetRestChildren
{
    public SyntaxKind Operator { get; } = op;
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [Type];
    }
}
