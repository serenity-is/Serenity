namespace Serenity.TypeScript;

public class MappedTypeNode(Token readonlyToken, TypeParameterDeclaration typeParameter, ITypeNode nameType,
        Token questionToken, ITypeNode type, NodeArray<ITypeElement> members)
    : Node(SyntaxKind.MappedType), ITypeNode, IDeclaration, IGetRestChildren
{
    public Token ReadonlyToken { get; } = readonlyToken;
    public TypeParameterDeclaration TypeParameter { get; } = typeParameter;
    public ITypeNode NameType { get; } = nameType;
    public Token QuestionToken { get; } = questionToken;
    public ITypeNode Type { get; } = type;
    public NodeArray<ITypeElement> Members { get; } = members;

    public IEnumerable<INode> GetRestChildren()
    {
        yield return ReadonlyToken;
        yield return TypeParameter;
        yield return NameType;
        yield return QuestionToken;
        yield return Type;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}