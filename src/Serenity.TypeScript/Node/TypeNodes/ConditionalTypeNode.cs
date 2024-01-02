namespace Serenity.TypeScript;

public class ConditionalTypeNode(ITypeNode checkType, ITypeNode extendsType,
    ITypeNode trueType, ITypeNode falseType)
    : TypeNodeBase(SyntaxKind.ConditionalType), IGetRestChildren
{
    public ITypeNode CheckType { get; } = checkType;
    public ITypeNode ExtendsType { get; } = extendsType;
    public ITypeNode TrueType { get; } = trueType;
    public ITypeNode FalseType { get; } = falseType;

    public IEnumerable<INode> GetRestChildren()
    {
        return [CheckType, ExtendsType, TrueType, FalseType];
    }
}
