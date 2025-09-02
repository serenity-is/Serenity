namespace Serenity.TypeScript;

public class JSDocPropertyTag : Node, IJSDocTag, ITypeElement, IHasNameProperty, IGetRestChildren
{
    public JSDocPropertyTag()
    {
        Kind = SyntaxKind.JSDocPropertyTag;
    }

    public JSDocTypeExpression TypeExpression { get; set; }
    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }
    public IDeclarationName Name { get; set; }
    public QuestionToken QuestionToken { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        return [TypeExpression, AtToken, TagName, AtToken, Name, QuestionToken];
    }
}
