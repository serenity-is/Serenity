namespace Serenity.TypeScript;

internal class ConstructSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.ConstructSignature, name: null,
        typeParameters, parameters, type), ISignatureDeclaration, ITypeElement
{
}
