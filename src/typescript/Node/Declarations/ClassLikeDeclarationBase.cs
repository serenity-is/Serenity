namespace Serenity.TypeScript;

public class ClassLikeDeclarationBase<TName>(SyntaxKind kind, TName name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : NamedDeclaration<TName>(kind, name), IClassLikeDeclaration, IGetRestChildren
    where TName : IDeclarationName
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<HeritageClause> HeritageClauses { get; } = heritageClauses;
    public NodeArray<IClassElement> Members { get; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (HeritageClauses != null) foreach (var x in HeritageClauses) yield return x;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}
