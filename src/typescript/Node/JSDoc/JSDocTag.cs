namespace Serenity.TypeScript;

public class JSDocTag(SyntaxKind kind = SyntaxKind.JSDocTag) : Node(kind), IJSDocTag, IGetRestChildren
{
    public AtToken? AtToken { get; set; }
    public Identifier? TagName { get; set; }
    public string? Comment { get; set; }

    public virtual IEnumerable<INode?> GetRestChildren()
    {
        return [AtToken, TagName];
    }
}
