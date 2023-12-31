namespace Serenity.TypeScript;

internal class CallSignatureDeclaration(NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : SignatureDeclarationBase(SyntaxKind.CallSignature, name: null,
        typeParameters, parameters, type), ISignatureDeclaration, ITypeElement
{
}
