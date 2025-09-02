namespace Serenity.TypeScript;
public class InterfaceDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<ITypeElement> members) : DeclarationStatement<Identifier>(SyntaxKind.InterfaceDeclaration, name),
        IHasModifiers, IGetRestChildren, IObjectTypeDeclaration, IDeclarationWithTypeParameterChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<HeritageClause> HeritageClauses { get; set; } = heritageClauses;
    public NodeArray<ITypeElement> Members { get; set; } = members;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (HeritageClauses != null) foreach (var x in HeritageClauses) yield return x;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}