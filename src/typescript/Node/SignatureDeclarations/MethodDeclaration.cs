namespace Serenity.TypeScript;

public class MethodDeclaration(NodeArray<IModifierLike> modifiers, AsteriskToken asteriskToken,
    IPropertyName name, QuestionToken questionToken, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, Block body)
        : FunctionLikeDeclarationBase(SyntaxKind.MethodDeclaration, name, typeParameters, parameters, type,
            asteriskToken: asteriskToken, questionToken: questionToken, body: body),
        IMethodOrAccessorDeclaration, IObjectLiteralElementLike, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
