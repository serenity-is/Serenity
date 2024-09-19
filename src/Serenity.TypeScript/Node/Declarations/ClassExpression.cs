namespace Serenity.TypeScript;

public class ClassExpression(NodeArray<IModifierLike> modifiers, Identifier name,
    NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<HeritageClause> heritageClauses,
    NodeArray<IClassElement> members)
     : ClassLikeDeclarationBase<Identifier>(SyntaxKind.ClassExpression, name, typeParameters, heritageClauses,
        members), IClassLikeDeclaration, IPrimaryExpression, IHasDecorators, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}