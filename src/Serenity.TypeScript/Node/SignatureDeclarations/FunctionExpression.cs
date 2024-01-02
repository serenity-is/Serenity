namespace Serenity.TypeScript;

public class FunctionExpression(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken,
    Identifier name, NodeArray<TypeParameterDeclaration> typeParameters, NodeArray<ParameterDeclaration> parameters,
    ITypeNode type, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.FunctionExpression, name, typeParameters, parameters,
        type, asteriskToken, body: body), IPrimaryExpression, IFunctionLikeDeclaration, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}