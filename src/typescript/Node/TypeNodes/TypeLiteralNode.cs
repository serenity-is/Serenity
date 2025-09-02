namespace Serenity.TypeScript;

public class TypeLiteralNode(NodeArray<ITypeElement> members)
    : TypeNodeBase(SyntaxKind.TypeLiteral), ITypeNode, IObjectTypeDeclaration, IHasNameProperty, IGetRestChildren
{
    public NodeArray<ITypeElement> Members { get; } = members;
    public IDeclarationName Name { get; }

    public IEnumerable<INode> GetRestChildren()
    {
        yield return Name;
        if (Members != null) foreach (var x in Members) yield return x;
    }
}