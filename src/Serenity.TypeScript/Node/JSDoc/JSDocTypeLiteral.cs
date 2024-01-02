namespace Serenity.TypeScript;

public class JSDocTypeLiteral() : JSDocTypeBase(SyntaxKind.JSDocTypeLiteral), IGetRestChildren
{
    public NodeArray<JSDocPropertyTag> JSDocPropertyTags { get; set; }
    public JSDocTypeTag JSDocTypeTag { get; set; }

    public IEnumerable<INode> GetRestChildren()
    {
        if (JSDocPropertyTags != null) foreach (var x in JSDocPropertyTags) yield return x;
        yield return JSDocTypeTag;
    }
}
