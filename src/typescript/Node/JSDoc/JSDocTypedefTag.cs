namespace Serenity.TypeScript;

public class JSDocTypedefTag : Node, IJSDocTag, IHasNameProperty, IGetRestChildren, IDeclarationWithTypeParameters
{
    public JSDocTypedefTag()
    {
        Kind = SyntaxKind.JSDocTypedefTag;
    }

    public INode FullName { get; set; } // JSDocNamespaceDeclaration | Identifier
    public JSDocTypeExpression TypeExpression { get; set; }
    public JSDocTypeLiteral JSDocTypeLiteral { get; set; }
    public IDeclarationName Name { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        return [FullName, TypeExpression, JSDocTypeLiteral, Name, AtToken, TagName];
    }
}
