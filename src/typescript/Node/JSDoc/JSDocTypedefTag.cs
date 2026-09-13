namespace Serenity.TypeScript;

public class JSDocTypedefTag() : Node(SyntaxKind.JSDocTypedefTag), IJSDocTag, IHasNameProperty, IGetRestChildren, IDeclarationWithTypeParameters
{
    public INode? FullName { get; set; } // JSDocNamespaceDeclaration | Identifier
    public JSDocTypeExpression? TypeExpression { get; set; }
    public JSDocTypeLiteral? JSDocTypeLiteral { get; set; }
    public IDeclarationName? Name { get; set; }
    public AtToken? AtToken { get; set; }
    public Identifier? TagName { get; set; }
    public string? Comment { get; set; }

    public IEnumerable<INode?> GetRestChildren()
    {
        return [FullName, TypeExpression, JSDocTypeLiteral, Name, AtToken, TagName];
    }
}
