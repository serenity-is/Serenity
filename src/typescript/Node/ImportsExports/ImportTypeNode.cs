namespace Serenity.TypeScript;

public class ImportTypeNode(ITypeNode argument, ImportAttributes attributes, IEntityName qualifier,
    NodeArray<ITypeNode> typeArguments, bool isTypeOf = false)
    : NodeWithTypeArguments(SyntaxKind.ImportType, typeArguments), IGetRestChildren
{
    public ITypeNode Argument { get; } = argument;
    public ImportAttributes Attributes { get; } = attributes;
    public IEntityName Qualifier { get; } = qualifier;
    public bool IsTypeOf { get; } = isTypeOf;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Argument;
        yield return Attributes;
        yield return Qualifier;
        foreach (var x in base.GetRestChildren())
            yield return x;
    }
}