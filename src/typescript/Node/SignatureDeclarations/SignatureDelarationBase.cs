namespace Serenity.TypeScript;

public class SignatureDeclarationBase(SyntaxKind kind, IDeclarationName name, NodeArray<TypeParameterDeclaration> typeParameters,
    NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    : NamedDeclaration<IDeclarationName>(kind, name), ISignatureDeclaration, IGetRestChildren
{
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; } = typeParameters;
    public NodeArray<ParameterDeclaration> Parameters { get; set; } = parameters;
    public ITypeNode Type { get; set; } = type;

    public override IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
    }
}
