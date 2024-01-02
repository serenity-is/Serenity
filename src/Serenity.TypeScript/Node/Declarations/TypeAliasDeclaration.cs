namespace Serenity.TypeScript;
public class TypeAliasDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, ITypeNode type)
    : DeclarationStatement<Identifier>(SyntaxKind.TypeAliasDeclaration, name), IHasModifiers, IGetRestChildren,
    IDeclarationWithTypeParameterChildren
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public ITypeNode Type { get; } = type;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        yield return Type;
    }
}