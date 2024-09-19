namespace Serenity.TypeScript;

public class TypeReferenceNode(IEntityName typeName, NodeArray<ITypeNode> typeArguments)
    : TypeNodeBase(SyntaxKind.TypeReference), IGetRestChildren
{
    public IEntityName TypeName { get; } = typeName;
    public NodeArray<ITypeNode> TypeArguments { get; } = typeArguments;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return TypeName;
        if (TypeArguments != null) foreach (var x in TypeArguments) yield return x;
    }
}
