namespace Serenity.TypeScript;

public class JSDocFunctionType(NodeArray<ParameterDeclaration>? parameters, ITypeNode? type) 
    : Node(SyntaxKind.JSDocFunctionType), IJSDocType, ISignatureDeclaration, IHasNameProperty, IGetRestChildren
{
    public NodeArray<ParameterDeclaration>? Parameters { get; set; } = parameters;
    public ITypeNode? Type { get; set; } = type;
    public IDeclarationName? Name { get; set; }
    public NodeArray<TypeParameterDeclaration>? TypeParameters { get; set; }

    public IEnumerable<INode?> GetRestChildren()
    {
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
    }
}
