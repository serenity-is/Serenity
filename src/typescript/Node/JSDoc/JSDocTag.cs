namespace Serenity.TypeScript;

public class JSDocTag : Node, IJSDocTag, IGetRestChildren
{
    public JSDocTag()
    {
        Kind = SyntaxKind.JSDocTag;
    }

    public AtToken AtToken { get; set; }
    public Identifier TagName { get; set; }
    public string Comment { get; set; }

    public virtual IEnumerable<INode> GetRestChildren()
    {
        return [AtToken, TagName];
    }
}
