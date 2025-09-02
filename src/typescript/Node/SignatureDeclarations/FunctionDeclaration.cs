namespace Serenity.TypeScript;

public class FunctionDeclaration(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken, 
    Identifier name, NodeArray<TypeParameterDeclaration> typeParameters, 
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.FunctionDeclaration, name, typeParameters, parameters, 
        type, asteriskToken: asteriskToken, body: body), IDeclarationStatement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
