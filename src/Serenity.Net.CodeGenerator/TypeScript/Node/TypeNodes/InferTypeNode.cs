namespace Serenity.TypeScript;

internal class InferTypeNode(TypeParameterDeclaration typeParameter)
    : TypeNodeBase(SyntaxKind.InferType), IInferTypeNode, IGetRestChildren
{
    public TypeParameterDeclaration TypeParameter { get; } = typeParameter;

    public IEnumerable<INode> GetRestChildren()
    {
        return [TypeParameter];
    }
}