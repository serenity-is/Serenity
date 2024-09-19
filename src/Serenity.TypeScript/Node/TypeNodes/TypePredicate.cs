namespace Serenity.TypeScript;

public class TypePredicateNode(AssertsKeyword assertsModifier, INode parameterName, ITypeNode type)
    : TypeNodeBase(SyntaxKind.TypePredicate), IGetRestChildren
{
    public AssertsKeyword AssertsModifier { get; } = assertsModifier;
    public INode ParameterName { get; } = parameterName; // Identifier | ThisTypeNode
    public ITypeNode Type { get; } = type;

    public IEnumerable<INode> GetRestChildren()
    {
        return [AssertsModifier, ParameterName, Type];
    }
}