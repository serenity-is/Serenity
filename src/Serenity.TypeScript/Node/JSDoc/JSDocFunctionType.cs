namespace Serenity.TypeScript;

public class JSDocFunctionType : Node, IJSDocType, ISignatureDeclaration, IHasNameProperty, IGetRestChildren
{
    public JSDocFunctionType(NodeArray<ParameterDeclaration> parameters, ITypeNode type)
    {
        Kind = SyntaxKind.JSDocFunctionType;
        Parameters = parameters;
        Type = type;
    }

    public NodeArray<ParameterDeclaration> Parameters { get; set; }
    public ITypeNode Type { get; set; }
    public IDeclarationName Name { get; set; }
    public NodeArray<TypeParameterDeclaration> TypeParameters { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (Parameters != null) foreach (var x in Parameters) yield return x;
        yield return Type;
        yield return Name;
        if (TypeParameters != null) foreach (var x in TypeParameters) yield return x;
    }
}
