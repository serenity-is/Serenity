namespace Serenity.TypeScript;

public class SetAccessorDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    NodeArray<ParameterDeclaration> parameters, Block body)
    : FunctionLikeDeclarationBase(SyntaxKind.SetAccessor, name, typeParameters: null,
        parameters, type: null, body: body), IAccessorDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
