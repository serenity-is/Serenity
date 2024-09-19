namespace Serenity.TypeScript;

public class ConstructorTypeNode(NodeArray<IModifierLike> modifiers, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : FunctionOrConstructorTypeNodeBase(SyntaxKind.ConstructorType, typeParameters, parameters, type), IHasModifiers
{
    public NodeArray<IModifierLike> Modifiers { get; } = modifiers;
}
