namespace Serenity.TypeScript;

public class GetAccessorDeclaration(NodeArray<IModifierLike> modifiers, IPropertyName name,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type, Block body) :
    FunctionLikeDeclarationBase(SyntaxKind.GetAccessor, name, typeParameters: null,
        parameters, type, body: body), IAccessorDeclaration, IHasModifiers, IHasDecorators
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
