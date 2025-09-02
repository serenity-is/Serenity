namespace Serenity.TypeScript;

public class ClassDeclaration(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
    : ClassLikeDeclarationBase<Identifier>(SyntaxKind.ClassDeclaration, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IDeclarationStatement, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
