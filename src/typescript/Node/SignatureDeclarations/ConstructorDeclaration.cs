namespace Serenity.TypeScript;

public class ConstructorDeclaration(NodeArray<IModifierLike> modifiers, NodeArray<ParameterDeclaration> parameters, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.Constructor, name: null, typeParameters: null, type: null,
        parameters: parameters, body: body), IClassElement, IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
